<template>
  <section>
    <pageHead>Your profile</pageHead>
    <div id="content-wrapper" class="site-content-wrapper site-pages" v-if="fetched">
      <div id="content" class="site-content layout-boxed">
        <div class="container">
          <form>
            <div class="row">
              <div class="col-xs-12 site-main-content">
                <main id="main" class="site-main">
                  <div class="white-box user-profile-wrapper">
                    <button type="button" @click="toggleEncrypted" id="toggle-encryption-btn">Show {{encrypted ? 'Plain Text' : 'Encrypted'}}</button>
                    <div class="row">
                      <div class="col-md-12" id="page-info">
                        <h1>Profile</h1>
                      </div>
                      <hr />
                    </div>

                    <div class="row info-section">
                      <h4 class="section-title">Personal information</h4>

                      <TideInput markup="firstName" class="col-md-4 col-sm-6" label="First Name" tidify="true" v-model="details.firstName" list="auto-fill-options" />
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
            fetched: false,
            encrypted: true,
            details: this.$config.scaffoldDetails
        };
    },
    async created() {
        try {
            this.fetched = false;
            this.$loading(true, "Fetching details");
            var data = (await this.$http.get(`${this.$tide.serverUrl}/application`)).data;
            if (!data.success) {
                this.$bus.$emit("show-message", "You must apply for a property before viewing your profile.");
                return this.$router.push("/apply");
            }
            this.details = data.content;
            this.fetched = true;
            this.$loading(false, "");
        } catch (error) {
            console.log(error);
        }
    },
    methods: {
        toggleEncrypted() {
            this.encrypted = !this.encrypted;
            this.$loading(true, this.encrypted ? "Encrypting..." : "Decrypting...");
            setTimeout(async () => {
                for (const property in this.details) {
                    if (property != "id" && property != "userId" && property != "dateSubmitted") {
                        this.details[property] = this.$tide[this.encrypted ? "encrypt" : "decrypt"](this.details[property], "field1");
                    }
                }
                this.$loading(false, "");
            }, 100);
        }
    }
};
</script>

<style lang="scss" scoped>
#toggle-encryption-btn {
    position: absolute;
    right: 25px;
    top: 10px;
}
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
