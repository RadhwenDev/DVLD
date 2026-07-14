using DVLD_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DVLD_BusinessLayer
{
    public class clsTestTypes
    {
        private clsTestTypes _OriginalTestTypes;
        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed
        }

        public int TestID { set; get; }
        public string Step { set; get; }
        public string Description { set; get; }
        public int Fees { set; get; }

        public clsTestTypes()
        {
            this.TestID = -1;
            this.Step = "";
            this.Description = "";
            this.Fees = 0;
        }

        public clsTestTypes(int TestID, string Step, string Description, int Fees)
        {
            this.TestID = TestID;
            this.Step = Step;
            this.Description = Description;
            this.Fees = Fees;

            _OriginalTestTypes = new clsTestTypes();
            _OriginalTestTypes.TestID = TestID;
            _OriginalTestTypes.Step = Step;
            _OriginalTestTypes.Description = Description;
            _OriginalTestTypes.Fees = Fees;
        }

        public static DataTable getAllTestTypes()
        {
            return clsTestTypesDataAccess.getAllTestTypes();
        }


        public enSaveResult UpdateTestType()
        {
            if (clsTestTypesDataAccess.UpdateTestTypes(this.TestID, this.Step, this.Description, this.Fees))
            {
                // تحديث النسخة الأصلية بالقيم الجديدة بعد الحفظ بنجاح
                _OriginalTestTypes.Step = this.Step;
                _OriginalTestTypes.Description = this.Description;
                _OriginalTestTypes.Fees = this.Fees;
                return enSaveResult.SavedSuccessfully;
            }
            return enSaveResult.Failed;
        }


        public enSaveResult Save()
        {
            if (!HasChanges())
                return enSaveResult.NoChanges;
            return UpdateTestType();
        }
        private bool HasChanges()
        {
            return
                TestID != _OriginalTestTypes.TestID ||
                Step != _OriginalTestTypes.Step ||
                Description != _OriginalTestTypes.Description ||
                Fees != _OriginalTestTypes.Fees;
        }
    }
}
