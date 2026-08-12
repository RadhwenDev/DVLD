using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BusinessLayer
{
    public class clsApplicationType
    {
        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed
        }

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        private clsApplicationType _OriginalApplicationType;

        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Fees { get; set; }

        public clsApplicationType()
        {
            this.ID = -1;
            this.Title = "";
            this.Fees = 0;

            Mode = enMode.AddNew;
        }

        private clsApplicationType(int ID, string Title, decimal Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Fees = Fees;

            // حفظ النسخة الأصلية للمقارنة
            _OriginalApplicationType = new clsApplicationType();
            _OriginalApplicationType.ID = ID;
            _OriginalApplicationType.Title = Title;
            _OriginalApplicationType.Fees = Fees;

            Mode = enMode.Update;
        }

        public static clsApplicationType Find(int ID)
        {
            string Title = "";
            decimal Fees = 0;

            if (clsApplicationTypesDataAccess.GetApplicationTypeInfoByID(ID, ref Title, ref Fees))
            {
                return new clsApplicationType(ID, Title, Fees);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetAllApplicationTypes()
        {
            return clsApplicationTypesDataAccess.GetAllApplicationTypes();
        }

        private bool HasChanges()
        {
            if (_OriginalApplicationType == null)
                return true;

            return (this.Title != _OriginalApplicationType.Title ||
                    this.Fees != _OriginalApplicationType.Fees);
        }

        private enSaveResult _UpdateApplicationType()
        {
            if (!HasChanges())
                return enSaveResult.NoChanges;

            if (clsApplicationTypesDataAccess.UpdateApplicationType(this.ID, this.Title, this.Fees))
            {
                // تحديث النسخة الأصلية بعد نجاح الحفظ
                _OriginalApplicationType.Title = this.Title;
                _OriginalApplicationType.Fees = this.Fees;

                return enSaveResult.SavedSuccessfully;
            }

            return enSaveResult.Failed;
        }

        public enSaveResult Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return enSaveResult.Failed;

                case enMode.Update:
                    return _UpdateApplicationType();
            }

            return enSaveResult.Failed;
        }
    }
}