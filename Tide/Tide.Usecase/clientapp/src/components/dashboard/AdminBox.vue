<template>
  <section id="panel-container">
    <div class="col-md-3 col-sm-6 col-xs-12">
      <div class="top-panel">
        <div class="panel-content">
          <img src="../../assets/img/dashboard/server.svg" alt="" />
          <div class="panel-info">
            <h4>Pending Deals</h4>
            <h2>{{ pendingDeals }}</h2>
          </div>
        </div>
        <div class="panel-footer">
          <i class="fa fa-refresh"></i> Last updated: {{ readableTime }}
        </div>
      </div>
    </div>
    <div class="col-md-3 col-sm-6 col-xs-12">
      <div class="top-panel">
        <div class="panel-content">
          <img src="../../assets/img/dashboard/wallet.svg" alt="" />
          <div class="panel-info">
            <h4>Tide</h4>
            <h2>{{ tide }}</h2>
          </div>
        </div>
        <div class="panel-footer">
          <i class="fa fa-refresh"></i> Last updated: {{ readableTime }}
        </div>
      </div>
    </div>
    <div class="col-md-3 col-sm-6 col-xs-12">
      <div class="top-panel">
        <div class="panel-content">
          <img src="../../assets/img/dashboard/error.svg" alt="" />
          <div class="panel-info">
            <h4>Approved Deals</h4>
            <h2>{{ approvedDeals }}</h2>
          </div>
        </div>
        <div class="panel-footer">
          <i class="fa fa-refresh"></i> Last updated: {{ readableTime }}
        </div>
      </div>
    </div>
    <div class="col-md-3 col-sm-6 col-xs-12">
      <div class="top-panel">
        <div class="panel-content">
          <img src="../../assets/img/dashboard/twitter.svg" alt="" />
          <div class="panel-info">
            <h4>Automate Deals</h4>
            <h2>{{ trustee ? "On" : "Off" }}</h2>
          </div>
        </div>
        <div class="panel-footer">
          <i
            class="fa"
            :class="{ 'on fa-check': trustee, 'off fa-times': !trustee }"
          ></i>
          <span id="toggle-trustee" @click="toggleTrustee">Toggle</span>
        </div>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  data() {
    return {
      lastUpdatedTide: 0,
      tide: 0,
      pendingDeals: 0,
      approvedDeals: 0,
      trustee: false,
      fetchCycle: 3,
      updateTimer: null,
      autoAcceptTimer: null
    }
  },
  async created() {
    if (this.$store.getters.user == null) return this.$router.push('/apply')
    setInterval(async () => {

      if (this.$store.getters.details != null) {
        this.$store.dispatch('getDeals', this.fetchCycle == 3);

        this.$store.commit('updateTide', this.$store.getters.user.tide)

        this.lastUpdatedTide = 0;
        this.tide = this.presentTide;
        this.pendingDeals = this.$store.getters.deals.length;
        this.approvedDeals = this.$store.getters.dealHistory.length;
      }
      this.fetchCycle++;
      if (this.fetchCycle >= 4) this.fetchCycle = 0;
    }, 2000);

    this.updateTimer = setInterval(() => {
      this.lastUpdatedTide++;
    }, 1000);

    this.autoAcceptTimer = setInterval(() => {
      if (this.$store.getters.user.trustee) this.$store.commit('autoAcceptDeal')

    }, 5000);

    this.trustee = this.$store.getters.user.trustee;
  },
  computed: {
    readableTime() {
      return this.$helper.secondsToString(this.lastUpdatedTide);
    },
    presentTide() {
      return this.$store.getters.user.tide.toFixed(2);
    }
  },
  methods: {
    async toggleTrustee() {
      this.$store.commit('updateTrustee', !this.$store.getters.user.trustee);
      this.trustee = this.$store.getters.user.trustee;
    }
  }
}
</script>

<style lang="scss" scoped>
#panel-container {
  width: 100%;
  margin-bottom: 20px;
}

.top-panel {
  height: 150px;
  margin-top: 30px;
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  border: 1px solid #e4e4e4;
}

.panel-content {
  height: 100px;
  padding: 15px;
  display: flex;
  flex-direction: row;
  align-items: center;
  img {
    width: 50px;
  }

  .panel-info {
    display: flex;
    flex-direction: column;
    justify-content: center;

    text-align: right;
    width: 100%;
    height: 100px;
    h4,
    h2 {
      margin: 0px;
    }
  }
}

.panel-footer {
  margin: auto 15px 0 15px;
  height: 50px;
  border-top: 3px solid #f1f1f1;
  display: flex;
  color: #a0a0a0;
  align-items: center;
  i {
    margin-right: 15px;
    margin-bottom: 2px;
  }
}

#toggle-trustee {
  color: #2291c5;
  cursor: pointer;
}

#toggle-trustee:hover {
  color: orange;
}
</style>
