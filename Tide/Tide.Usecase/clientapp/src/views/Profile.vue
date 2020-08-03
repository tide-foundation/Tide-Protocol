<template>
  <section>
    <pageHead>
      Your profile
    </pageHead>
    <div id="content-wrapper" class="site-content-wrapper site-pages">
      <div id="content" class="site-content layout-boxed">
        <div class="container">
          <form @submit.prevent="submit">
            <div class="row">
              <div class="col-xs-12 site-main-content">
                <main id="main" class="site-main">
                  <div class="white-box user-profile-wrapper">
                    <div class="row">
                      <div class="col-md-12" id="page-info">
                        <h1>Profile</h1>
                      </div>
                      <hr />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Personal information</h4>
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="First Name"
                        tidify="true"
                        required="required"
                        v-model="details.personal.first"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Middle Name"
                        tidify="true"
                        v-model="details.personal.middle"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Last Name"
                        tidify="true"
                        required="required"
                        v-model="details.personal.last"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Email"
                        tidify="true"
                        required="required"
                        v-model="details.personal.email"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Phone Number"
                        tidify="true"
                        required="required"
                        v-model="details.personal.phone"
                      />

                      <TideInput
                        :classification="classification.age"
                        class="col-md-4 col-sm-6"
                        label="Date of Birth"
                        tidify="true"
                        required="required"
                        v-model="details.personal.date"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Previous addresses</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Address"
                        tidify="true"
                        required="required"
                        v-model="details.addresses.current.address"
                      />
                      <TideInput
                        :classification="classification.location"
                        class="col-md-3 col-sm-6"
                        label="Suburb"
                        required="required"
                        v-model="details.addresses.current.suburb"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="State"
                        required="required"
                        v-model="details.addresses.current.state"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Postcode"
                        required="required"
                        v-model="details.addresses.current.postcode"
                      />

                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Address"
                        tidify="true"
                        required="required"
                        v-model="details.addresses.previous.address"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Suburb"
                        required="required"
                        v-model="details.addresses.previous.suburb"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="State"
                        required="required"
                        v-model="details.addresses.previous.state"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Postcode"
                        required="required"
                        v-model="details.addresses.previous.postcode"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Employment information</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Employer"
                        required="required"
                        v-model="details.employment.current.employer"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Phone"
                        required="required"
                        tidify="true"
                        v-model="details.employment.current.phone"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Email"
                        tidify="true"
                        required="required"
                        v-model="details.employment.current.email"
                      />
                      <TideInput
                        :classification="classification.earn"
                        class="col-md-3 col-sm-6"
                        label="Monthly Pay"
                        required="required"
                        v-model="details.employment.current.pay"
                      />
                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Employer"
                        required="required"
                        v-model="details.employment.previous.employer"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Phone"
                        tidify="true"
                        required="required"
                        v-model="details.employment.previous.phone"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Email"
                        tidify="true"
                        required="required"
                        v-model="details.employment.previous.email"
                      />
                      <TideInput
                        class="col-md-3 col-sm-6"
                        label="Monthly Pay"
                        required="required"
                        v-model="details.employment.previous.pay"
                      />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">
                        Credit history
                        <span
                          class="classification"
                          v-if="classification != null"
                          >({{ classification.debt }})</span
                        >
                      </h4>

                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Credit card outstanding"
                        required="required"
                        tidify="true"
                        v-model="details.credit.creditCard"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Peronal loan outstanding"
                        tidify="true"
                        v-model="details.credit.personalLoan"
                      />
                      <TideInput
                        class="col-md-4 col-sm-6"
                        label="Other loan outstanding"
                        required="required"
                        tidify="true"
                        v-model="details.credit.otherLoan"
                      />
                    </div>

                    <button
                      id="submit-changes-btn"
                      v-if="$store.getters.tideEngaged && !submitting"
                    >
                      Submit Changes
                    </button>
                  </div>
                </main>
              </div>
            </div>
          </form>
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
      inputs: [],
      user: this.$store.getters.user,
      details: this.$config.scaffoldDetails,
      classification: {},
      submitting: false
    };
  },
  async created() {
    this.details = JSON.parse(this.$store.getters.details.details);
    this.classification = this.$store.getters.details.classified;
    this.$loading(false, "");
    this.$bus.$on("toggle-tide-start", on => {
      if (!on) this.classification = this.$helper.classifyData(this.details);
    });
  },

  methods: {
    async submit() {
      try {
        this.submitting = true;
        if (this.$store.getters.tideEngaged) {
          this.$bus.$emit("toggle-tide");
          await this.$helper.sleep(1000);
        }

        this.$loading(
          true,
          "Encrypting your data and storing it with the vendor."
        );

        setTimeout(async () => {
          this.$store.commit("storeDetails", {
            details: JSON.stringify(this.details),
            classified: this.classification
          });

          this.$loading(false, "");
          this.$bus.$emit(
            "show-message",
            "Your information has been successfully submitted"
          );
          this.submitting = false;
        }, 2000);
      } catch (thrownError) {
        this.$bus.$emit("show-error", thrownError);
        this.$loading(false, "");
        this.submitting = false;
      }
    }
  }
};
</script>

<style lang="scss" scoped>
#submit-changes-btn {
  background: #03a9f4;
  border: 2px solid #03a9f4;
  color: white;
  position: relative;
  z-index: 300 !important;
  width: 100%;
  height: 40px;
  margin: 10px 0;
}

#submit-changes-btn:hover {
  border: 2px solid white;
}

h2 {
  color: #0dbae8;
}

.info-section {
  pointer-events: none;
}

.faded {
  opacity: 0.5;
}

.disable {
  pointer-events: none;
}

#profile-deal-btn {
  margin-bottom: 30px;
  span {
    color: #466bc6;
  }
}

.classification {
  font-size: 14px;
}
</style>
