using DVLD_Business;
using DVLD_BusinessLayer;
using DVLD_PresentationLayer.Global;
using DVLD_PresentationLayer.Licenses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLD_BusinessLayer.clsLicenses;
using static DVLD_PresentationLayer.Licenses.ucLicenses;

namespace DVLD_PresentationLayer.Applications
{
    public partial class ucThirdStepNewApp : UserControl
    {
        public event EventHandler<int> DataBack;
        public int SelectedPersonID { get; private set; } = -1;
        public int SelectedApplicationTypeID { get; private set; } = -1;
        public int SelectedLicenseClassID { get; private set; } = -1;

        public event EventHandler OnStepThirdCompleted;
        public event EventHandler OnBackButtonClicked;

        public ucThirdStepNewApp()
        {
            InitializeComponent();
        }

        public ucThirdStepNewApp(int personID, int applicationTypeID, int licenseClassID) : this()
        {
            SelectedPersonID = personID;
            SelectedApplicationTypeID = applicationTypeID;
            SelectedLicenseClassID = licenseClassID;
        }
        decimal fees = 0;
        private void ucThirdStepNewApp_Load(object sender, EventArgs e)
        {
            DataTable _dtName = clsPerson.GetFullNameByID(SelectedPersonID);
            DataTable _dtAppType = clsApplicant.getApplicationTypesTitle_Fees(SelectedApplicationTypeID);
            lblApplicant.Text = _dtName.Rows[0]["FullName"].ToString();
            lblService.Text = _dtAppType.Rows[0]["ApplicationTypeTitle"].ToString();
            if (SelectedLicenseClassID == -1)
                label7.Visible = false;
            else
            {
                label7.Visible = true;
                DataTable _dtLicenseClass = clsLicenseClass.GetLicenseClassesNameByID(SelectedLicenseClassID);
                lblLicenseClass.Text = _dtLicenseClass.Rows[0]["ClassName"].ToString();
            }
            fees = Convert.ToDecimal(_dtAppType.Rows[0]["ApplicationFees"]);
            lblFees.Text = $"${fees:N2}";
            lblDate.Text = DateTime.Now.ToString("MMMM dd, yyyy");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            OnBackButtonClicked?.Invoke(this, e);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            byte ApplicationStatus = 1;
            if (SelectedApplicationTypeID != 1)
            {
                ApplicationStatus = 3;
            }
            int ApplicationID = clsApplicant.AddNewApplication(SelectedPersonID, DateTime.Now, SelectedApplicationTypeID, ApplicationStatus, DateTime.Now, fees, clsCurrentUser._UserID);
            if (ApplicationID != -1)
            {
                MessageBox.Show($"Application saved successfully with ID = {ApplicationID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (SelectedApplicationTypeID == 1 || SelectedApplicationTypeID == 7)
                {
                    int LocalDrivingLicenseAppID = clsLocalDrivingLicenseApplications.AddNewLocalDrivingLicenseApplications(ApplicationID, SelectedLicenseClassID);
                    if(LocalDrivingLicenseAppID != -1)
                    {
                        MessageBox.Show($"Local Driving License Applications saved successfully with ID = {LocalDrivingLicenseAppID}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save Local Driving License Application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (SelectedApplicationTypeID == 2) // Renew Driving License
                {
                    // 1. التثبت من الـ LicenseClass وجلب آخر رخصة خاصة بهذا الصنف تحديداً
                    SelectedLicenseClassID = clsLicenseClass.GetLicenseClassIDByPersonID(SelectedPersonID);
                    clsLicenses oldLicense = clsLicenses.FindLastLicenseByPersonIDAndClass(SelectedPersonID, SelectedLicenseClassID);

                    if (oldLicense == null)
                    {
                        MessageBox.Show("No license found for this person with the selected License Class!",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. التثبت أن الرخصة من نفس الصنف المحدد (إضافي للأمان)
                    if (oldLicense.LicenseClass != SelectedLicenseClassID)
                    {
                        MessageBox.Show("The retrieved license class does not match the selected license class!",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. التحقق مما إذا كانت الرخصة محجوزة
                    if (clsDetainedLicense.IsLicenseDetained(oldLicense.LicenseID))
                    {
                        MessageBox.Show("This license is currently detained! You must release it before renewing.",
                                        "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 4. التحقق من حالة الرخصة (يلزم تكون Active لكي تُجدد)
                    if (!oldLicense.IsActive)
                    {
                        MessageBox.Show("This license is inactive and cannot be renewed!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 5. التحقق من انتهاء الصلاحية
                    if (oldLicense.ExpirationDate > DateTime.Now)
                    {
                        MessageBox.Show($"This license is still valid until {oldLicense.ExpirationDate.ToShortDateString()} and cannot be renewed yet!",
                                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 6. تأكيد التجديد من المستخدم
                    DialogResult result = MessageBox.Show($"Are you sure you want to renew License ID [{oldLicense.LicenseID}] for Class [{oldLicense.LicenseClass}]?",
                                                  "Confirm Renewal",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }


                    // 9. إلغاء تفعيل الرخصة القديمة (Deactivate)
                    if (!clsLicenses.Deactivate(oldLicense.LicenseID))
                    {
                        MessageBox.Show("Failed to deactivate the old license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 10. إنشاء وتفعيل الرخصة الجديدة بنفس الـ LicenseClass
                    clsLicenses newLicense = new clsLicenses();
                    newLicense.ApplicationID = ApplicationID;
                    newLicense.DriverID = oldLicense.DriverID;
                    newLicense.LicenseClass = oldLicense.LicenseClass; // إسناد صنف الرخصة القديمة
                    newLicense.IssueDate = DateTime.Now;

                    int defaultValidityYears = clsLicenseClass.GetDefaultValidityLength(oldLicense.LicenseClass);
                    newLicense.ExpirationDate = DateTime.Now.AddYears(defaultValidityYears > 0 ? defaultValidityYears : 10);

                    newLicense.Notes = oldLicense.Notes;
                    newLicense.PaidFees = clsLicenseClass.GetClassFees(oldLicense.LicenseClass);
                    newLicense.IsActive = true;
                    newLicense.IssueReason = clsLicenses.enIssueReason.Renew;
                    newLicense.CreatedByUserID = clsCurrentUser._UserID;

                    if (newLicense.Save())
                    {
                        MessageBox.Show($"License renewed successfully!\nNew License ID: {newLicense.LicenseID}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to issue the new license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (SelectedApplicationTypeID == 3 || SelectedApplicationTypeID == 4)
                {
                    // 1. جلب صنف الرخصة والرخصة القديمة
                    SelectedLicenseClassID = clsLicenseClass.GetLicenseClassIDByPersonID(SelectedPersonID);
                    clsLicenses oldLicense = clsLicenses.FindLastLicenseByPersonIDAndClass(SelectedPersonID, SelectedLicenseClassID);

                    if (oldLicense == null)
                    {
                        MessageBox.Show("No license found for this person with the selected License Class!",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 2. التثبت أن الرخصة من نفس الصنف المحدد
                    if (oldLicense.LicenseClass != SelectedLicenseClassID)
                    {
                        MessageBox.Show("The retrieved license class does not match the selected license class!",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. التحقق مما إذا كانت الرخصة محجوزة
                    if (clsDetainedLicense.IsLicenseDetained(oldLicense.LicenseID))
                    {
                        MessageBox.Show("This license is currently detained! You must release it before replacing.",
                                        "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 4. التحقق من حالة الرخصة (يلزم تكون Active)
                    if (!oldLicense.IsActive)
                    {
                        MessageBox.Show("This license is inactive and cannot be replaced!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 5. التحقق من انتهاء الصلاحية
                    if (oldLicense.ExpirationDate < DateTime.Now)
                    {
                        MessageBox.Show("This license is expired! You must renew it instead of replacing it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 6. تحديد نوع الطلب وسبب الإصدار والرسالة المناسبة
                    string issueReasonText = ((enReplacementReason)SelectedApplicationTypeID == enReplacementReason.Damaged) ? "Damaged" : "Lost";

                    // 🎯 تصليح الرسالة: تغيير Renew إلى Replace
                    DialogResult result = MessageBox.Show($"Are you sure you want to replace License ID [{oldLicense.LicenseID}] (Reason: Replacement for {issueReasonText})?",
                                                   "Confirm Replacement",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    // 7. تحديد نوع الـ Application والـ IssueReason
                    clsApplicant.enApplicationType appType;
                    clsLicenses.enIssueReason issueReason;

                    if ((enReplacementReason)SelectedApplicationTypeID == enReplacementReason.Damaged)
                    {
                        appType = clsApplicant.enApplicationType.ReplaceDamagedDrivingLicense;
                        issueReason = clsLicenses.enIssueReason.ReplacementForDamaged;
                    }
                    else
                    {
                        appType = clsApplicant.enApplicationType.ReplaceLostDrivingLicense;
                        issueReason = clsLicenses.enIssueReason.ReplacementForLost;
                    }

                    // 8. جلب رسوم طلب البدل
                    DataTable dtAppType = clsApplicant.getApplicationTypesTitle_Fees((int)appType);
                    if (dtAppType == null || dtAppType.Rows.Count == 0)
                    {
                        MessageBox.Show("Failed to load application fees.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    decimal replacementAppFees = Convert.ToDecimal(dtAppType.Rows[0]["ApplicationFees"]);

                    // 10. إلغاء تفعيل الرخصة القديمة (Deactivate)
                    if (!clsLicenses.Deactivate(oldLicense.LicenseID))
                    {
                        MessageBox.Show("Failed to deactivate the old license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 11. إنشاء وتفعيل الرخصة الجديدة
                    clsLicenses newLicense = new clsLicenses();
                    newLicense.ApplicationID = ApplicationID;
                    newLicense.DriverID = oldLicense.DriverID;
                    newLicense.LicenseClass = oldLicense.LicenseClass;
                    newLicense.IssueDate = DateTime.Now;

                    // ⚠️ في بدل التالف/المفقود نحافظ على نفس تاريخ الانتهاء القديم
                    newLicense.ExpirationDate = oldLicense.ExpirationDate;

                    newLicense.Notes = oldLicense.Notes;
                    newLicense.PaidFees = 0; // عادة تكون رسوم إصدار بدل التالف/المفقود للرخصة 0 لأن الرسوم تُدفع في الـ Application
                    newLicense.IsActive = true;
                    newLicense.IssueReason = issueReason;
                    newLicense.CreatedByUserID = clsCurrentUser._UserID;

                    if (newLicense.Save())
                    {
                        // 🎯 تصليح الرسالة النهائية
                        MessageBox.Show($"License replaced successfully!\nNew License ID: {newLicense.LicenseID}",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to issue the replacement license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (SelectedApplicationTypeID == 5)
                {
                    clsDetainedLicense detainedLicense = clsDetainedLicense.FindByPersonID(SelectedPersonID);
                    if (detainedLicense == null || detainedLicense.IsReleased)
                    {
                        MessageBox.Show("No active detained license found for this person.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult result = MessageBox.Show($"Are you sure you want to release Detained License ID [{detainedLicense.DetainID}]?",
                                                  "Confirm Release",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        return;
                    clsLicenses license = clsLicenses.Find(detainedLicense.LicenseID);
                    if (license == null)
                    {
                        MessageBox.Show("Associated license details not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 6. تحديث سجل الحجز وتحويله إلى Released
                    detainedLicense.IsReleased = true;
                    detainedLicense.ReleaseDate = DateTime.Now;
                    detainedLicense.ReleasedByUserID = clsCurrentUser._UserID;
                    detainedLicense.ReleaseApplicationID = ApplicationID;

                    if (detainedLicense.Save())
                    {
                        MessageBox.Show($"Detained License released successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show("Failed to update detained record status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

                else if (SelectedApplicationTypeID == 6)
                {
                    clsInternationalLicense internationalLicense = new clsInternationalLicense();

                    // تعبئة البيانات المطلوبة
                    internationalLicense.ApplicationID = ApplicationID;
                    clsDriver driver = clsDriver.FindByPersonID(SelectedPersonID);
                    if (driver == null)
                    {
                        MessageBox.Show("Failed to find this Driver record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int activeLocalLicenseID = clsLicenses.GetLicenseIDByApplicationID(driver.DriverID);

                    if (activeLocalLicenseID == -1)
                    {
                        MessageBox.Show("No active local license found for this person to issue an international license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    internationalLicense.DriverID = driver.DriverID; // تأكد من الحصول على DriverID المطلوب
                    internationalLicense.IssuedUsingLocalLicenseID = activeLocalLicenseID; // ID الرخصة المحلية المستند عليها
                    internationalLicense.IssueDate = DateTime.Now;
                    internationalLicense.ExpirationDate = DateTime.Now.AddYears(1); // عادة مدة الرخصة الدولية سنة
                    internationalLicense.IsActive = true;
                    internationalLicense.CreatedByUserID = clsCurrentUser._UserID;

                    if (internationalLicense.Save())
                    {
                        MessageBox.Show($"International License saved successfully with ID = {internationalLicense.InternationalLicenseID}!",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save International License data.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                    // 🌟 إرسال الـ ID عبر الـ DataBack المخصص للإرسال الخارجي
                    DataBack?.Invoke(this, ApplicationID);

                // 🌟 إطلاق حدث اكتمال الخطوة الثالثة ليتحرك الـ Wizard
                OnStepThirdCompleted?.Invoke(this, EventArgs.Empty);
                btnContinue.Enabled = false;
                btnBack.Enabled = false;
            }
            else
            {
                MessageBox.Show("Failed to save Application data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
