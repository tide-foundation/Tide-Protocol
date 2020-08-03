export default class TideHelper {
  constructor(tide, bus, store) {
    this.characters = "a b c d e f g h i j k l m n o p q r s t u v w x y z A B C D E F G H I J K L M N O P Q R S U V W X Y Z 1 2 3 4 5 6 7 8 9 0 ! @ # $ % ^ & * ( ) _ + - =".split(
      " "
    );
    this.tide = tide;
    this.bus = bus;
    this.store = store;
    this.event = new Event("input");
  }

  getTideInputs() {
    return document.getElementsByClassName("tide-input-main");
  }

  async toggleTide() {
    console.log(this.tide);
    const inputs = this.getTideInputs();
    return new Promise(async resolve => {
      this.bus.$emit("toggle-tide-start", !this.store.getters.tideEngaged);

      this.store.commit("updateTideProcessing", true);
      this.store.commit("updateTideEngaged", !this.store.getters.tideEngaged);
      if (this.store.getters.tideEngaged) {
        this.bus.$emit("tint", true);
      }
      for (let input of inputs) {
        if (this.store.getters.tideEngaged) {
          this.bus.$emit("engage-input", {
            key: input.name,
            val: true
          });

          this.scatterReveal(input, this.tide.decrypt(input.value));
        } else {
          this.bus.$emit("engage-input", {
            key: input.name,
            val: false
          });

          this.scatterReveal(input, this.tide.encrypt(input.value));
        }
        await this.sleep(70);
      }
      this.store.commit("updateTideProcessing", false);
      if (!this.store.getters.tideEngaged) this.bus.$emit("tint", false);
      return resolve();
    });
  }

  async scatterReveal(input, endResult) {
    if (!input.classList.contains("tidify")) return;
    var length = input.value.length;
    this.bus.$emit("update-lock-start", {
      key: input.name,
      val: !this.store.getters.tideEngaged
    });
    for (var i = 0; i < 20; i++) {
      if (length < endResult.length) length++;
      else if (length > endResult.length) length--;
      this.shuffleArray(this.characters);
      input.value = this.characters.slice(0, length).join("");
      await this.sleep(20);
    }
    input.value = endResult;
    input.dispatchEvent(this.event);
    this.bus.$emit("update-lock-end", {
      key: input.name,
      val: !this.store.getters.tideEngaged
    });
  }

  shuffleArray(array) {
    for (var i = array.length - 1; i > 0; i--) {
      var j = Math.floor(Math.random() * (i + 1));
      var t = array[i];
      array[i] = array[j];
      array[j] = t;
    }
  }

  sleep(m) {
    return new Promise(r => setTimeout(r, m));
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
      location: details.addresses.current.suburb,
      earn: this.getAmountBracket(parseInt(details.employment.current.pay)),
      debt: this.getAmountBracket(
        parseInt(details.credit.creditCard) +
          parseInt(details.credit.personalLoan) +
          parseInt(details.credit.otherLoan)
      ),
      age: this.getAgeBracket(details.personal.date)
    };
    return data;
  }

  secondsToString(seconds) {
    var numminutes = Math.floor((((seconds % 31536000) % 86400) % 3600) / 60);
    var numseconds = (((seconds % 31536000) % 86400) % 3600) % 60;
    return numminutes + " m " + numseconds + " s";
  }
}
