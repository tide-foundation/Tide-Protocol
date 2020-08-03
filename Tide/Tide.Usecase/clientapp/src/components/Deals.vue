<template>
  <div id="deals-container" v-if="show">
    <section id="content">
      <section id="header">
        <h4>Deals</h4>

        <img
          @click="show = false"
          src="../assets/img/cancel.svg"
          alt="Cancel btn"
        />
      </section>
      <section id="available-deals">
        <div id="trustee-box">
          <div class="form-check" style="width:100%;text-align:left">
            <input
              type="checkbox"
              id="trustee-checkbox"
              :checked="$store.getters.details.trustee"
              @click="toggleTrustee"
            />
            <label for="trustee-checkbox">Automated trustee</label>
          </div>
        </div>
        <table>
          <thead>
            <tr>
              <th>Seeker</th>
              <th>Query</th>
              <th>Fields</th>
              <th>TIDE</th>
              <th></th>
            </tr>
          </thead>

          <tbody id="deal-history-table">
            <tr v-for="deal in $store.getters.deals" :key="deal.id">
              <td class="force-width">
                <img
                  :src="require(`../assets/img/logo/${deal.seeker}.png`)"
                  alt=""
                />
                <span class="hidden-sm hidden-xs">{{ deal.seeker }}</span>
              </td>
              <td>{{ deal.query }}</td>
              <td>{{ deal.fields.join(", ") }}</td>
              <td>{{ formatTide((45 / 100) * deal.deal_value) }}</td>
              <td style="padding:0">
                <button
                  :class="deal.request.state + '-btn'"
                  @click="acceptDeal(deal)"
                >
                  {{
                    deal.request.state == "Pending"
                      ? "Approve"
                      : deal.request.state
                  }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
        <div class="no-deals-note" v-if="$store.getters.deals.length == 0">
          No current deals
        </div>
      </section>

      <section id="previous-deals" class="overflow nice-scroll">
        <h4>Previous Deals</h4>

        <table>
          <thead>
            <tr>
              <th style="width:12%">Seeker</th>
              <th style="width:12%">ID</th>

              <th>Query</th>
              <th style="width:15%">TIDE</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="deal in $store.getters.dealHistory" :key="deal.id">
              <td class="force-width">
                <img
                  style="width:70px"
                  :src="require(`../assets/img/logo/${deal.seeker}.png`)"
                  alt=""
                />
                <span class="hidden-sm hidden-xs">{{ deal.seeker }}</span>
              </td>
              <td>{{ deal.dealId }}</td>

              <td>{{ deal.query }}</td>
              <td>{{ formatTide((45 / 100) * deal.tide) }}</td>
            </tr>
          </tbody>
        </table>
        <div
          class="no-deals-note"
          v-if="$store.getters.dealHistory.length == 0"
        >
          No current deals
        </div>
      </section>
    </section>
  </div>
</template>

<script>
export default {
  components: {},
  data() {
    return {
      show: false
    };
  },
  async created() {
    this.$bus.$on("show-deals", async () => {
      if (this.$store.getters.user == null) {
        this.$store.commit("updateRoute", {
          action: "event",
          value: "show-deals"
        });
        this.$bus.$emit("show-message", "Please login or register to continue");
        return this.$bus.$emit("showLoginModal", true);
      }

      if (this.$store.getters.details == null) {
        try {
          this.$loading(true, "Fetching your settings");
          if (this.$store.getters.details == null) {
            this.$bus.$emit(
              "show-message",
              "Please apply for a property before accessing your deals"
            );
            this.$router.push("/apply");
            this.$loading(false, "");
            return;
          }
        } catch (errorThrown) {
          this.$bus.$emit("show-error", errorThrown);
        }

        this.$loading(false, "");
      }

      this.show = true;
    });
  },
  computed: {
    getButtonText(state) {
      switch (state) {
        case "Pending":
          return "Approve";
        case "Approved":
          return "Approved";
        case "Processing":
          return "Processing";
      }
      return "Approve";
    }
  },
  methods: {
    formatTide(val) {
      return `Ŧ ${parseFloat(val).toFixed(2)}`;
    },
    acceptDeal(deal) {
      this.$store.commit("updateDeal", {
        id: deal.id,
        state: "Processing"
      });
      this.$store.dispatch("acceptDeal", { deal: deal, state: "Approved" });
    },
    async toggleTrustee() {
      this.$store.commit("updateTrustee", !this.$store.getters.user.trustee);
      this.trustee = this.$store.getters.user.trustee;
    }
  }
};
</script>

<style lang="scss" scoped>
#deals-container {
  text-align: center;
  z-index: 999;
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  width: 100%;
  height: 100%;
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background-color: rgba(0, 0, 0, 0.5);
  background-color: red;
}

#content {
  width: 90%;

  height: 90%;
  background-color: #f0f1f2;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

#available-deals {
  width: 100%;
}

#previous-deals {
  margin-top: 20px;
  flex-grow: 2;
  width: 100%;
  height: 60%;
}

#header {
  height: 45px !important;
  width: 100%;
  background-color: #252525;
  text-align: left;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  h4 {
    font-weight: normal;
    margin: 0px 15px;
  }
}

#deals-content {
  padding: 10px;
  text-align: left;
  flex-grow: 2;
}

img {
  cursor: pointer;
  width: 20px;
  margin-right: 10px;
}

table {
  border-spacing: -1;
  margin: 10px;
  width: calc(100% - 20px);
}

th {
  text-align: left;
  padding-left: 5px;
  border: 0px solid #252525;
}

td {
  text-align: left;
  padding: 0px 5px;
  border: 1px solid #252525;
}

tr {
  height: 40px;
}

tr:nth-child(even) {
  background-color: #dddddd;
}

button {
  width: 100px;
  margin: 0px;
  height: 40px;
  background-color: #01acfe;
  border: 0px solid transparent;
  color: white;
}

button:hover {
  background-color: #556270;
  color: white;
}

.Approved-btn {
  background-color: #a7d129;
  pointer-events: none;
}

.Denied-btn {
  background-color: #cf3030;
  pointer-events: none;
}

.Processing-btn {
  background-color: #556270;
  pointer-events: none;
}

tbody {
  font-size: 12px;
  @media screen and (max-width: 800px) {
    font-size: 10px;
  }
}

table {
  @media screen and (max-width: 350px) {
    margin: 3px;
    width: calc(100% - 6px);
  }
}

button {
  @media screen and (max-width: 800px) {
    width: 50px;
  }
}

#content {
  @media screen and (max-width: 600px) {
    width: 95%;
  }

  @media screen and (max-width: 350px) {
    width: 98%;
  }
}

.no-deals-note {
  margin: 0 auto;
  text-align: center;
}

.force-width {
  min-width: 185px;
  @media screen and (max-width: 990px) {
    min-width: auto;
  }
}

.force-width {
  background-color: red;
}

#trustee-box {
  margin: 15px;
}

.overflow {
  overflow-y: scroll;
}
</style>
