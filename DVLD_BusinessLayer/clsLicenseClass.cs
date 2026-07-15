using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DVLD_BusinessLayer
{
    public class clsLicenseClass
    {
        private clsLicenseClass _OriginalLicenseClass;

        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed
        }
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int LicenseClassID { set; get; }
        public string ClassName { set; get; }
        public string ClassDescription { set; get; }
        public int MinimumAge { set; get; }
        public int Validity { set; get; }
        public int Fees { set; get; }

        public clsLicenseClass()
        {
            this.LicenseClassID = -1;
            this.ClassName = "";
            this.ClassDescription = "";
            this.MinimumAge = 18;
            this.Validity = 0;
            this.Fees = 0;
            Mode = enMode.AddNew;
        }

        public clsLicenseClass(int LicenseClassID, string ClassName, string ClassDescription, int MinimumAge, int Validity, int Fees)
        {
            this.LicenseClassID = LicenseClassID;
            this.ClassName = ClassName;
            this.ClassDescription = ClassDescription;
            this.MinimumAge = MinimumAge;
            this.Validity = Validity;
            this.Fees = Fees;

            _OriginalLicenseClass = new clsLicenseClass();
            _OriginalLicenseClass.LicenseClassID = LicenseClassID;
            _OriginalLicenseClass.ClassName = ClassName;
            _OriginalLicenseClass.ClassDescription = ClassDescription;
            _OriginalLicenseClass.MinimumAge = MinimumAge;
            _OriginalLicenseClass.Validity = Validity;
            _OriginalLicenseClass.Fees = Fees;

            Mode = enMode.Update;
        }

        public static DataTable GetAllLicenseClasses()
        {
            return clsLicenseClassDataAccess.GetAllLicenseClasses();
        }

        public enSaveResult UpdateTestType()
        {
            if (clsLicenseClassDataAccess.UpdateLicenseClasses(this.LicenseClassID, this.ClassName, this.ClassDescription, this.MinimumAge, this.Validity, this.Fees))
            {
                // تحديث النسخة الأصلية بالقيم الجديدة بعد الحفظ بنجاح
                _OriginalLicenseClass.ClassName = this.ClassName;
                _OriginalLicenseClass.ClassDescription = this.ClassDescription;
                _OriginalLicenseClass.MinimumAge = this.MinimumAge;
                _OriginalLicenseClass.Validity = this.Validity;
                _OriginalLicenseClass.Fees = this.Fees;
                return enSaveResult.SavedSuccessfully;
            }
            return enSaveResult.Failed;
        }

        public static clsLicenseClass Find(int LicenseClassID)
        {
            string ClassName = "", ClassDescription = "";
            int MinimumAge = 0, Validity = 0, Fees = 0;
            if (clsLicenseClassDataAccess.Find(LicenseClassID, ref ClassName, ref ClassDescription, ref MinimumAge, ref Validity, ref Fees))
                return new clsLicenseClass(LicenseClassID, ClassName, ClassDescription, MinimumAge, Validity, Fees);
            return null;
        }

        public bool _AddNewLicenseClass()
        {
            this.LicenseClassID = clsLicenseClassDataAccess.AddNewLicenseClass(
                this.ClassName,
                this.ClassDescription,
                this.MinimumAge,
                this.Validity,
                this.Fees
            );

            return (this.LicenseClassID != -1);
        }


        public enSaveResult Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewLicenseClass())
                    {
                        // تحويل الحالة إلى Update وتجهيز الكائن الأصلي للمستقبل
                        Mode = enMode.Update;

                        _OriginalLicenseClass = new clsLicenseClass();
                        _OriginalLicenseClass.LicenseClassID = this.LicenseClassID;
                        _OriginalLicenseClass.ClassName = this.ClassName;
                        _OriginalLicenseClass.ClassDescription = this.ClassDescription;
                        _OriginalLicenseClass.MinimumAge = this.MinimumAge;
                        _OriginalLicenseClass.Validity = this.Validity;
                        _OriginalLicenseClass.Fees = this.Fees;

                        return enSaveResult.SavedSuccessfully;
                    }
                    else
                    {
                        return enSaveResult.Failed;
                    }

                case enMode.Update:
                    if (!HasChanges())
                        return enSaveResult.NoChanges;

                    return UpdateTestType();
            }

            return enSaveResult.Failed;
        }
        private bool HasChanges()
        {
            return
                LicenseClassID != _OriginalLicenseClass.LicenseClassID ||
                ClassName != _OriginalLicenseClass.ClassName ||
                ClassDescription != _OriginalLicenseClass.ClassDescription ||
                MinimumAge != _OriginalLicenseClass.MinimumAge ||
                Validity != _OriginalLicenseClass.Validity ||
                Fees != _OriginalLicenseClass.Fees;
        }
    }
}
