export default {
  orkNodes: [
    "https://tide-ork-02.azurewebsites.net/",
    "https://tide-ork-01.azurewebsites.net/",
    "https://tide-ork-03.azurewebsites.net/",
    "https://tide-ork-04.azurewebsites.net/",
    "https://tide-ork-05.azurewebsites.net/"
  ],
  blockchainUrl: "https://jungle2.cryptolions.io:443",
  chainId: "e70aaab8997e1dfce58fbfac80cbbb8fecec7b99cf982a9444273cbc64c41473",
  onboardingContract: "onboardingxj",
  transactionContract: "transaction3",
  currencyContract: "tidecurrency",
  vendorContract: "tidevendorzz",
  vendorAccount: "futurehomesx",

  scaffoldDetails: {
    personal: {
      first: "",
      middle: "",
      last: "",
      phone: "",
      email: "",
      date: ""
    },
    addresses: {
      current: {
        address: "",
        suburb: "",
        state: "",
        postcode: ""
      },
      previous: {
        address: "",
        suburb: "",
        state: "",
        postcode: ""
      }
    },
    employment: {
      current: {
        employer: "",
        phone: "",
        email: "",
        pay: ""
      },
      previous: {
        employer: "",
        phone: "",
        email: "",
        pay: ""
      }
    },
    credit: {
      creditCard: "",
      personalLoan: "",
      otherLoan: ""
    }
  },

  mockData: [
    {
      personal: {
        first: "Eli",
        middle: "Olsen",
        last: "Rodriquez",
        phone: "041-706-0644",
        email: "eli.rodriquez@example.com",
        date: "1985-04-29"
      },
      addresses: {
        current: {
          address: "6684 frueløkke",
          suburb: "Ansager",
          state: "Nordjylland",
          postcode: "79782"
        },
        previous: {
          address: "5569 tunalı hilmi cd",
          suburb: "Malatya",
          state: "Tunceli",
          postcode: "76537"
        }
      },
      employment: {
        current: {
          employer: "Ziva Co",
          phone: "5529 7718",
          email: "info@ziva.co",
          pay: "4650"
        },
        previous: {
          employer: "Saffron Carpets",
          phone: "5812 3627",
          email: "info@saffron",
          pay: "2920"
        }
      },
      credit: {
        creditCard: "5400",
        personalLoan: "15600",
        otherLoan: "3500"
      }
    },
    {
      personal: {
        first: "Timmothy",
        middle: "John",
        last: "Gray",
        phone: "081-713-4854",
        email: "tim@saffroncarpets.com",
        date: "1977-02-05"
      },
      addresses: {
        current: {
          address: "1012 Westmoreland Street",
          suburb: "Ansager",
          state: "Roscommon",
          postcode: "65946"
        },
        previous: {
          address: "4850 Lone Wolf Trail",
          suburb: "Norwalk",
          state: "Florida",
          postcode: "99536"
        }
      },
      employment: {
        current: {
          employer: "Blacklace co",
          phone: "5529 3318",
          email: "info@blacklace.co",
          pay: "5250"
        },
        previous: {
          employer: "Mcdonalds",
          phone: "NA",
          email: "management.ansager@mcdonalds.com",
          pay: "2620"
        }
      },
      credit: {
        creditCard: "2000",
        personalLoan: "0",
        otherLoan: "7500"
      }
    }
  ],
  getNextDeal() {
    var deal = dealTypes[currentDealIndex];
    deal.id = currentDealIndex;
    currentDealIndex++;
    if (currentDealIndex >= dealTypes.length) currentDealIndex = 0;

    deal.accepted = false;
    return deal;
  }
};

var currentDealIndex = 0;
const dealTypes = [
  {
    seeker: "ingxxxxxtide",
    query: "ING wants your mobile number to call you about contents insurance",
    value: 0.165,
    fields: ["Mobile Number"]
  },
  {
    seeker: "attxxxxxtide",
    query:
      "AT&T wants your household information and email address to send you a sign-up special offer",
    value: 0.08,
    fields: ["Household Info", "Email"]
  },
  {
    seeker: "maseratitide",
    query:
      "Maserati Co wants to access your credit score and mailing address to invite you to test drive their new convertible ",
    value: 100,
    fields: ["Credit Score", "Home Address"]
  },
  {
    seeker: "britishgasxx",
    query:
      "British Gas wants to access your age and number of household members to offer you a sign-up incentive",
    value: 0.215,
    fields: ["Date of Birth", "Number of residents"]
  },
  {
    seeker: "papajohnsxxx",
    query:
      "Pappa Johns wants your mobile number to shout you your first pizza in the neighbourhood",
    value: 0.06,
    fields: ["Mobile Number"]
  },
  {
    seeker: "gartnerxtide",
    query:
      "Gartner wants access to your employer records for a new study on mid management",
    value: 0.66,
    fields: ["Employer Records"]
  },
  {
    seeker: "airtaskerxxx",
    query:
      "Airtasker wants your location and email to invite you to compare removalists in your area",
    value: 0.05,
    fields: ["Address", "Email"]
  },
  {
    seeker: "allianzxxxxx",
    query:
      "Allianz wants your email and credit rating to invite you read about a tailored contents insurance offer",
    value: 0.11,
    fields: ["Email", "Credit Rating"]
  },
  {
    seeker: "supercleanxx",
    query:
      "Super Clean, your local Laundromat wants your mobile number to offer 50% off your first visit",
    value: 0.04,
    fields: ["Mobile Number"]
  },
  {
    seeker: "valiantxxxxx",
    query:
      "Valiant wants your email, household details and income to introduce a personalised furniture hire package",
    value: 0.25,
    fields: ["Email", "Household Info", "Income"]
  }
];
