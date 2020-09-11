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
                  <form autocomplete="off" class="submit-form" @submit.prevent="submit">
                    <div class="row">
                      <div class="col-md-12" id="page-info">
                        <h2>Rental Application</h2>
                        <h4>Villa in Coral Gables</h4>
                      </div>
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Personal information</h4>
                      <TideInput markup="firstName" class="col-md-4 col-sm-6" label="First Name" tidify="true" v-model="details.firstName" list="auto-fill-options" />

                      <datalist id="auto-fill-options">
                        <option value="Eli" />
                        <option value="Timmothy" />
                      </datalist>
                      <TideInput markup="middleName" class="col-md-4 col-sm-6" label="Middle Name" tidify="true" v-model="details.middleName" />
                      <TideInput markup="lastName" class="col-md-4 col-sm-6" label="Last Name" tidify="true" v-model="details.lastName" />
                      <TideInput markup="email" class="col-md-4 col-sm-6" label="Email" tidify="true" v-model="details.email" />
                      <TideInput markup="phoneNumber" class="col-md-4 col-sm-6" label="Phone Number" tidify="true" v-model="details.phoneNumber" />

                      <TideInput markup="dateOfBirth" class="col-md-4 col-sm-6" label="Date of Birth" tidify="true" v-model="details.dateOfBirth" />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Previous addresses</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput markup="currentAddress" class="col-md-3 col-sm-6" label="Address" tidify="true" v-model="details.currentAddress" />
                      <TideInput markup="currentSuburb" class="col-md-3 col-sm-6" label="Suburb" v-model="details.currentSuburb" />
                      <TideInput markup="currentState" class="col-md-3 col-sm-6" label="State" v-model="details.currentState" />
                      <TideInput markup="currentPostcode" class="col-md-3 col-sm-6" label="Postcode" v-model="details.currentPostcode" />

                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput markup="previousAddress" class="col-md-3 col-sm-6" label="Address" tidify="true" v-model="details.previousAddress" />
                      <TideInput markup="previousSuburb" class="col-md-3 col-sm-6" label="Suburb" v-model="details.previousSuburb" />
                      <TideInput markup="previousState" class="col-md-3 col-sm-6" label="State" v-model="details.previousState" />
                      <TideInput markup="previousPostcode" class="col-md-3 col-sm-6" label="Postcode" v-model="details.previousPostcode" />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Employment information</h4>
                      <h5 class="section-subtitle">Current</h5>
                      <TideInput markup="currentEmployer" class="col-md-3 col-sm-6" label="Employer" v-model="details.currentEmployer" />
                      <TideInput markup="currentEmployerPhone" class="col-md-3 col-sm-6" label="Phone" tidify="true" v-model="details.currentEmployerPhone" />
                      <TideInput markup="currentEmployerEmail" class="col-md-3 col-sm-6" label="Email" tidify="true" v-model="details.currentEmployerEmail" />
                      <TideInput markup="currentMonthlyPay" class="col-md-3 col-sm-6" label="Monthly Pay" v-model="details.currentMonthlyPay" />
                      <h5 class="section-subtitle">Previous</h5>
                      <TideInput markup="previousEmployer" class="col-md-3 col-sm-6" label="Employer" v-model="details.previousEmployer" />
                      <TideInput markup="previousEmployerPhone" class="col-md-3 col-sm-6" label="Phone" tidify="true" v-model="details.previousEmployerPhone" />
                      <TideInput markup="previousEmployerEmail" class="col-md-3 col-sm-6" label="Email" tidify="true" v-model="details.previousEmployerEmail" />
                      <TideInput markup="previousMonthlyPay" class="col-md-3 col-sm-6" label="Monthly Pay" v-model="details.previousMonthlyPay" />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Credit history</h4>
                      <TideInput markup="creditCardOutstanding" class="col-md-4 col-sm-6" label="Credit card outstanding" tidify="true" v-model="details.creditCardOutstanding" />
                      <TideInput markup="personalLoanOutstanding" class="col-md-4 col-sm-6" label="Peronal loan outstanding" tidify="true" v-model="details.personalLoanOutstanding" />
                      <TideInput markup="otherLoanOutstanding" class="col-md-4 col-sm-6" label="Other loan outstanding" tidify="true" v-model="details.otherLoanOutstanding" />
                    </div>

                    <div id="disclaimer-checkbox-container" class="form-check" style="width:100%;text-align:left">
                      <input type="checkbox" id="disclaimer-checkbox" :value="accepted" :checked="accepted" @click="accepted = !accepted" />
                      <label for="disclaimer-checkbox">
                        I hereby accept
                        <span class="disclaimer-link">Future Places Terms and Conditions</span>
                      </label>
                    </div>

                    <button :class="{ 'btn-disabled': !accepted }" v-if="!submitting" id="submit-btn" type="submit">Submit application</button>
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
    created() {
        this.$loading(false, "");
    },
    watch: {
        "details.firstName": function(newVal) {
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
                    if (markup != "firstName") {
                        this.populateField(input, mockData[markup]);
                    }
                }
            }
        }
    },
    methods: {
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

                this.$loading(true, "Encrypting your data and storing it with Future Places.");
                setTimeout(async () => {
                    try {
                        var encryptedObject = {};
                        for (const property in this.details) {
                            encryptedObject[property] = this.$tide.encrypt(this.details[property], "field1");
                        }

                        var result = (await this.$http.post(`http://127.0.0.1:6001/application`, encryptedObject)).data;

                        this.submitting = false;
                        this.$bus.$emit("show-message", "Your application has been submitted successfully");
                        this.$bus.$emit("update-menu", { name: "Thanks", route: "/thanks" });
                    } catch (thrownError) {}
                }, 100);
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
