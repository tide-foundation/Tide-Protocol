<template>
  <section id="deals-container">
    <div class="box" id="pending">
      <table>
        <tr>
          <th>Pending Deals</th>
          <th></th>
          <th></th>
          <th class="hidden-md hidden-sm hidden-xs"></th>
          <th></th>
        </tr>
        <tr v-for="(deal, index) in $store.getters.deals" :key="index">
          <td class="force-width seeker-col">
            <img
              class="seeker-img"
              :src="require(`../../assets/img/logo/${deal.seeker}.png`)"
              alt=""
            />
          </td>

          <td class="query-col">{{ deal.query }}</td>
          <td class="hidden-md hidden-sm hidden-xs">
            {{ deal.fields.join(", ") }}
          </td>
          <td class="tide-col">
            {{ formatTide(deal.value) }}
          </td>
          <td class="id-col">
            <button
              :class="deal.accepted ? 'approved-btn' : 'approve-btn'"
              @click="acceptDeal(deal)"
            >
              {{ deal.accepted ? "Approved" : "Approve" }}
            </button>
          </td>
        </tr>
      </table>
    </div>

    <div class="box" id="history">
      <table>
        <thead>
          <tr class="green">
            <th>Deal History</th>
            <th></th>
            <th></th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(deal, index) in $store.getters.dealHistory" :key="index">
            <td class="force-width seeker-col">
              <img
                class="seeker-img"
                :src="require(`../../assets/img/logo/${deal.seeker}.png`)"
                alt=""
              />
            </td>

            <td class="query-col">{{ deal.query }}</td>
            <td class="tide-col">{{ formatTide(deal.value) }}</td>
            <td class="id-col">{{ deal.dealId }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script>
export default {
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
      this.$store.commit("acceptDeal", deal);
    }
  }
};
</script>

<style lang="scss" scoped>
table {
  width: 100%;

  font-size: 12px;
  background: #f8fafc;
}

th,
td {
  border: 0px;
  border-bottom: #d3dce2 1px solid;
}

td {
  height: 50px;
}

.seeker-img {
  width: 100px;
}

button {
  width: 100%;
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

.approved-btn {
  background-color: #aee239;
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

.id-col,
.seeker-col {
  width: 112px;
}

.tide-col {
  width: 80px;
}

#pending {
  border-top: 2px solid orange;
}

#history {
  border-top: 2px solid #8fbe00;
  margin-top: 30px;
}
</style>
