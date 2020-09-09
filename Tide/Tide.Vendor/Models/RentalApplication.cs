using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.Vendor.Models
{
    public class RentalApplication
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Vuid
        public DateTimeOffset DateSubmitted { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }

        public string CurrentAddress { get; set; }
        public string CurrentSuburb { get; set; }
        public string CurrentState { get; set; }
        public string CurrentPostcode { get; set; }

        public string PreviousAddress { get; set; }
        public string PreviousSuburb { get; set; }
        public string PreviousState { get; set; }
        public string PreviousPostcode { get; set; }

        public string CurrentEmployer { get; set; }
        public string CurrentEmployerPhone { get; set; }
        public string CurrentEmployerEmail { get; set; }
        public string CurrentMonthlyPay { get; set; }

        public string PreviousEmployer { get; set; }
        public string PreviousEmployerPhone { get; set; }
        public string PreviousEmployerEmail { get; set; }
        public string PreviousMonthlyPay { get; set; }

        public string CreditCardOutstanding { get; set; }
        public string PersonalLoanOutstanding { get; set; }
        public string OtherLoanOutstanding { get; set; }

    }
}
