<template>
  <section>
    <pageHead>Apply for Villa in Coral Gables</pageHead>
    <div id="content-wrapper" class="site-content-wrapper site-pages">
      <div id="content" class="site-content layout-boxed">
        <div class="container">
          <div class="row">
            <div class="col-xs-12 site-main-content">
              <main id="main" class="site-main">
                <div class="white-box user-profile-wrapper">
                  <form
                    autocomplete="off"
                    class="submit-form"
                    @submit.prevent="submit"
                  >
                    <div class="row">
                      <div class="col-md-12" id="page-info">
                        <h2>Rental Application</h2>
                        <h4>Villa in Coral Gables</h4>
                      </div>
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Personal information</h4>
                      <TideInput
                        markup="personal.first"
                        class="col-md-4 col-sm-6"
                        label="First Name"
                        tidify="true"
                        v-model="details.personal.first"
                        list="auto-fill-options"
                      />

                      <datalist id="auto-fill-options">
                        <option value="Eli" />
                        <option value="Timmothy" />
                      </datalist>

                      <TideInput
                        markup="personal.middle"
                        class="col-md-4 col-sm-6"
                        label="Middle Name"
                        tidify="true"
                        v-model="details.personal.middle"
                      />
                      <TideInput
                        markup="personal.last"
                        class="col-md-4 col-sm-6"
                        label="Last Name"
                        tidify="true"
                        v-model="details.personal.last"
                      />
                      <TideInput
                        markup="personal.email"
                        class="col-md-4 col-sm-6"
                        label="Email"
                        tidify="true"
                        v-model="details.personal.email"
                      />
                      <TideInput
                        markup="personal.phone"
                        class="col-md-4 col-sm-6"
                        label="Phone Number"
                        tidify="true"
                        v-model="details.personal.phone"
                      />

                      <TideInput
                        markup="personal.date"
                        class="col-md-4 col-sm-6"
                        label="Date of Birth"
                        tidify="true"
                        v-model="details.personal.date"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Previous addresses</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput
                        markup="addresses.current.address"
                        class="col-md-3 col-sm-6"
                        label="Address"
                        tidify="true"
                        v-model="details.addresses.current.address"
                      />
                      <TideInput
                        markup="addresses.current.suburb"
                        class="col-md-3 col-sm-6"
                        label="Suburb"
                        v-model="details.addresses.current.suburb"
                      />
                      <TideInput
                        markup="addresses.current.state"
                        class="col-md-3 col-sm-6"
                        label="State"
                        v-model="details.addresses.current.state"
                      />
                      <TideInput
                        markup="addresses.current.postcode"
                        class="col-md-3 col-sm-6"
                        label="Postcode"
                        v-model="details.addresses.current.postcode"
                      />

                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput
                        markup="addresses.previous.address"
                        class="col-md-3 col-sm-6"
                        label="Address"
                        tidify="true"
                        v-model="details.addresses.previous.address"
                      />
                      <TideInput
                        markup="addresses.previous.suburb"
                        class="col-md-3 col-sm-6"
                        label="Suburb"
                        v-model="details.addresses.previous.suburb"
                      />
                      <TideInput
                        markup="addresses.previous.state"
                        class="col-md-3 col-sm-6"
                        label="State"
                        v-model="details.addresses.previous.state"
                      />
                      <TideInput
                        markup="addresses.previous.postcode"
                        class="col-md-3 col-sm-6"
                        label="Postcode"
                        v-model="details.addresses.previous.postcode"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Employment information</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput
                        markup="employment.current.employer"
                        class="col-md-3 col-sm-6"
                        label="Employer"
                        v-model="details.employment.current.employer"
                      />
                      <TideInput
                        markup="employment.current.phone"
                        class="col-md-3 col-sm-6"
                        label="Phone"
                        tidify="true"
                        v-model="details.employment.current.phone"
                      />
                      <TideInput
                        markup="employment.current.email"
                        class="col-md-3 col-sm-6"
                        label="Email"
                        tidify="true"
                        v-model="details.employment.current.email"
                      />
                      <TideInput
                        markup="employment.current.pay"
                        class="col-md-3 col-sm-6"
                        label="Monthly Pay"
                        v-model="details.employment.current.pay"
                      />
                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput
                        markup="employment.previous.employer"
                        class="col-md-3 col-sm-6"
                        label="Employer"
                        v-model="details.employment.previous.employer"
                      />
                      <TideInput
                        markup="employment.previous.phone"
                        class="col-md-3 col-sm-6"
                        label="Phone"
                        tidify="true"
                        v-model="details.employment.previous.phone"
                      />
                      <TideInput
                        markup="employment.previous.email"
                        class="col-md-3 col-sm-6"
                        label="Email"
                        tidify="true"
                        v-model="details.employment.previous.email"
                      />
                      <TideInput
                        markup="employment.previous.pay"
                        class="col-md-3 col-sm-6"
                        label="Monthly Pay"
                        v-model="details.employment.previous.pay"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Credit history</h4>
                      <TideInput
                        markup="credit.creditCard"
                        class="col-md-4 col-sm-6"
                        label="Credit card outstanding"
                        tidify="true"
                        v-model="details.credit.creditCard"
                      />
                      <TideInput
                        markup="credit.personalLoan"
                        class="col-md-4 col-sm-6"
                        label="Peronal loan outstanding"
                        tidify="true"
                        v-model="details.credit.personalLoan"
                      />
                      <TideInput
                        markup="credit.otherLoan"
                        class="col-md-4 col-sm-6"
                        label="Other loan outstanding"
                        tidify="true"
                        v-model="details.credit.otherLoan"
                      />
                    </div>

                    <div
                      id="disclaimer-checkbox-container"
                      class="form-check"
                      style="width:100%;text-align:left"
                    >
                      <input
                        type="checkbox"
                        id="disclaimer-checkbox"
                        :value="accepted"
                        :checked="accepted"
                        @click="accepted = !accepted"
                      />
                      <label for="disclaimer-checkbox">
                        I hereby accept
                        <span class="disclaimer-link"
                          >Future Places Terms and Conditions</span
                        >
                      </label>
                    </div>

                    <button
                      :class="{ 'btn-disabled': !accepted }"
                      v-if="!submitting"
                      id="submit-btn"
                      type="submit"
                    >
                      Submit application
                    </button>
                  </form>
                </div>
              </main>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
<script>
import pageHead from "../components/PageHead.vue";
import TideInput from "../components/TideInput.vue";

export default {
  components: {
    pageHead,
    TideInput
  },
  data() {
    return {
      accepted: false,
      inputs: this.$helper.getTideInputs(),
      event: new Event("input"),
      details: this.$config.scaffoldDetails,
      submitting: false,
      classification: {},
      glow: false
    };
  },
  mounted() {
    // this.$bus.$on('init', () => this.init());
    // this.$bus.$on('submit', () => this.submit());
    // this.$bus.$on('submit-details', () => this.submit());
    // this.$bus.$on('toggle-tide-start', (on) => {
    //   if (!on) this.classification = this.$helper.classifyData(this.details);
    // })
  },
  watch: {
    "details.personal.first": function(newVal) {
      if (this.$store.getters.tideProcessing) return;
      var mockData;
      switch (newVal) {
        case "Eli":
          mockData = this.$config.mockData[0];
          break;
        case "Timmothy":
          mockData = this.$config.mockData[1];
          break;
      }

      if (mockData != null) {
        for (let input of this.inputs) {
          const markup = input.getAttribute("markup");
          if (markup != "personal.first") {
            var splitMarkup = markup.split(".");
            if (splitMarkup.length == 2)
              this.populateField(
                input,
                mockData[splitMarkup[0]][splitMarkup[1]]
              );
            else
              this.populateField(
                input,
                mockData[splitMarkup[0]][splitMarkup[1]][splitMarkup[2]]
              );
          }
        }
      }
    }
  },
  methods: {
    async init() {
      this.$bus.$emit("tint", true);
      this.$store.commit("updateTideEngaged", true);

      for (let input of this.inputs) {
        this.$bus.$emit("update-lock-start", {
          key: input.name,
          val: !this.$store.getters.tideEngaged
        });
        this.$bus.$emit("engage-input", { key: input.name, val: true });
        this.$bus.$emit("update-lock-end", {
          key: input.name,
          val: !this.$store.getters.tideEngaged
        });
      }
    },
    async populateField(input, endResult) {
      const currentLength = input.value.length;

      // Remove current field
      for (var i = 0; i < currentLength; i++) {
        input.value = input.value.slice(0, input.value.length - 1);
        await this.$helper.sleep(20);
      }

      // Add new field
      for (var j = -1; j < endResult.length; j++) {
        input.value += endResult.slice(j, j + 1);
        await this.$helper.sleep(20);
      }

      input.dispatchEvent(this.event);
    },
    async submit() {
      try {
        if (this.$store.getters.user == null) {
          this.$bus.$emit("show-auth", true);
          return;
        }
        this.submitting = true;
        if (this.$store.getters.tideEngaged) {
          await this.$helper.toggleTide(this.$store.getters.user.keys.vendor);
        }

        this.$loading(
          true,
          "Encrypting your data and storing it with Future Places."
        );

        setTimeout(async () => {
          this.$store.commit("storeDetails", {
            details: JSON.stringify(this.details),
            classified: this.classification
          });

          this.submitting = false;
          this.$bus.$emit(
            "show-message",
            "Your application has been submitted successfully"
          );
          this.$bus.$emit("update-menu", { name: "Thanks", route: "/thanks" });
        }, 2000);
      } catch (thrownError) {
        this.submitting = false;
        this.$bus.$emit("show-error", thrownError);
        this.$loading(false, "");
      }
    }
  }
};
</script>

<style lang="scss" scoped>
#submit-btn {
  background: #03a9f4;
  border: 2px solid #03a9f4;
  color: white;
  position: relative;
  z-index: 300 !important;
  width: 100%;
  height: 40px;
  margin: 30px 0 0 0;
}

#disclaimer-checkbox-container {
  z-index: 700 !important;
}

#submit-btn:hover {
  border: 2px solid white;
}

h2 {
  color: #0dbae8;
}

#home-img {
  border: 2px solid black;
}

#update-user {
  background-color: orange !important;
}

.btn-disabled {
  pointer-events: none;
  opacity: 0.5;
}

.disclaimer-link {
  color: #03a9f4;
  cursor: pointer;
}

.disclaimer-link:hover {
  color: orange;
}
</style>
