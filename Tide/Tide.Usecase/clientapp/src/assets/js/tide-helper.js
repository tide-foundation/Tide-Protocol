export default class TideHelper {
  constructor() {
    this.event = new Event("input");
  }

  getTideInputs() {
    return document.getElementsByClassName("tide-input-main");
  }

  sleep(m) {
    return new Promise((r) => setTimeout(r, m));
  }

  calculateAge(date) {
    var birthday = new Date(date);
    var ageDifMs = Date.now() - birthday.getTime();
    var ageDate = new Date(ageDifMs);
    return Math.abs(ageDate.getUTCFullYear() - 1970);
  }

  getAgeBracket(age) {
    try {
      age = this.calculateAge(age);
      if (age < 10) return "0 - 9";
      else if (age >= 10 && age < 20) return "10 - 19";
      else if (age >= 20 && age < 30) return "20 - 29";
      else if (age >= 30 && age < 40) return "30 - 39";
      else if (age >= 40 && age < 50) return "40 - 49";
      else if (age >= 50 && age < 60) return "50 - 59";
      else if (age >= 60 && age < 70) return "60 - 69";
      else if (age >= 70 && age < 80) return "70 - 79";
      else if (age >= 80 && age < 90) return "80 - 89";
      else return "90 +";
    } catch (error) {
      return "Could not classify";
    }
  }

  getAmountBracket(amount) {
    try {
      if (amount < 10000) return "$0 - $9999";
      if (amount >= 10000 && amount < 50000) return "$10k - $49k";
      if (amount >= 50000 && amount < 100000) return "$50k - $99k";
      if (amount >= 100000 && amount < 250000) return "$100k - $249k";
      if (amount >= 250000 && amount < 1000000) return "$250k - $1m";
      return "$1,000,000 +";
    } catch (error) {
      return "Could not classify";
    }
  }

  classifyData(details) {
    const data = {
      location: details.currentSuburb,
      earn: this.getAmountBracket(parseInt(details.currentMonthlyPay)),
      debt: this.getAmountBracket(parseInt(details.creditCardOutstanding) + parseInt(details.personalLoanOutstanding) + parseInt(details.otherLoanOutstanding)),
      age: this.getAgeBracket(details.dateOfBirth),
    };
    return data;
  }
}
