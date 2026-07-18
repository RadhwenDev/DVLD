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
    public class clsPerson
    {
        private clsPerson _OriginalPerson;
        public enum enSaveResult
        {
            SavedSuccessfully,
            NoChanges,
            Failed
        }
        enum enMode { AddNew, Update };
        enMode Mode;
        public int PersonID {  set; get; }
        public string NationalNo { set; get; }
        public string FirstName { set; get; }
        public string SecondName { set; get; }
        public string ThirdName { set; get; }
        public string LastName { set; get; }
        public DateTime DateOfBirth { set; get; }
        public byte Gendor {  set; get; }
        public string Address { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public int NationalCountryID { set; get; }
        public string ImagePath { set; get; }


        public clsPerson()
        {
            this.PersonID = -1;
            this.NationalNo = "";
            this.FirstName = "";
            this.SecondName = "";
            this.ThirdName = "";
            this.LastName = "";
            this.DateOfBirth = DateTime.Now;
            this.Gendor = 0;
            this.Address = "";
            this.Phone = "";
            this.Email = "";
            this.NationalCountryID = -1;
            this.ImagePath = "";
            Mode = enMode.AddNew;
        }


        public clsPerson(int PersonID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName, DateTime DateOfBirth, byte Gendor, string Address, string Phone, string Email, int NationalCountryID, string ImagePath)
        {
            this.PersonID = PersonID;
            this.NationalNo = NationalNo;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Gendor = Gendor;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalCountryID = NationalCountryID;
            this.ImagePath = ImagePath;
            Mode = enMode.Update;
            _OriginalPerson = (clsPerson)this.MemberwiseClone();
        }

        public static DataTable GetPeople()
        {
            return clsPeopleDataAccess.GetPeople();
        }


        public static DataTable GetPeopleFullName()
        {
            return clsPeopleDataAccess.GetPeopleFullName();
        }

        public static DataTable GetPeopleAplicationFullName()
        {
            return clsPeopleDataAccess.GetPeopleAplicationFullName();
        }

        public static DataTable GetFullNameByID(int PersonID)
        {
            return clsPeopleDataAccess.GetFullNameByID(PersonID);
        }

        public bool AddNewPerson()
        {
            this.PersonID = clsPeopleDataAccess.AddNewPerson(this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName, this.DateOfBirth, this.Gendor, this.Address, this.Phone, this.Email, this.NationalCountryID, this.ImagePath);
            return (this.PersonID != -1);
        }
        public enSaveResult UpdatePerson()
        {
            if (clsPeopleDataAccess.UpdatePerson(this.PersonID, this.NationalNo, this.FirstName, this.SecondName, this.ThirdName, this.LastName, this.DateOfBirth, this.Gendor, this.Address, this.Phone, this.Email, this.NationalCountryID, this.ImagePath))
                return enSaveResult.SavedSuccessfully;
            return enSaveResult.Failed;
        }

        public static clsPerson Find(int PersonID)
        {
            string NationalNo = "", FirstName = "", SecondName = "", ThirdName = "", LastName = "", Address = "", Phone = "", Email = "", ImagePath = "";
            byte Gendor = 0;
            int NationalCountryID = -1;
            DateTime DateOfBirth = DateTime.Now;
            if (clsPeopleDataAccess.GetPeopleInfoByID(PersonID, ref NationalNo, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Gendor, ref Address, ref Phone, ref Email,
                ref NationalCountryID, ref ImagePath))
                return new clsPerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName, DateOfBirth, Gendor, Address, Phone, Email, NationalCountryID, ImagePath);
            return null;
        }


        public enSaveResult Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return enSaveResult.SavedSuccessfully;
                    }
                    else
                    {
                        return enSaveResult.Failed;
                    }
                case enMode.Update:
                    if (!HasChanges())
                        return enSaveResult.NoChanges;

                    return UpdatePerson();

            }
            return enSaveResult.Failed;
        }
        public static bool IsPersonExist(string NationalNo)
        {
            return clsPeopleDataAccess.IsPersonExist(NationalNo);
        }

        private bool HasChanges()
        {
            return
                FirstName != _OriginalPerson.FirstName ||
                SecondName != _OriginalPerson.SecondName ||
                ThirdName != _OriginalPerson.ThirdName ||
                LastName != _OriginalPerson.LastName ||
                NationalNo != _OriginalPerson.NationalNo ||
                Phone != _OriginalPerson.Phone ||
                Email != _OriginalPerson.Email ||
                Address != _OriginalPerson.Address ||
                DateOfBirth != _OriginalPerson.DateOfBirth.Date ||
                Gendor != _OriginalPerson.Gendor ||
                NationalCountryID != _OriginalPerson.NationalCountryID ||
                ImagePath != _OriginalPerson.ImagePath;
        }
    }
}
