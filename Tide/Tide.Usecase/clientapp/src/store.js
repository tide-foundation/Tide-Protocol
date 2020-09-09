import Vue from "vue";
import Vuex from "vuex";
import config from "./assets/js/config";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    loading: {
      active: false,
      text: "Loading...",
    },
    error: "Blockchain account failed to be created",
    user: null,
    application: null,
    details: null,
    classified: null,
    network: null,
    deals: [],
    dealHistory: [],
    tide: 0,
    route: "",
    delegatedTrustee: false,
  },
  mutations: {
    updateLoading(state, data) {
      state.loading = data;
    },
    updateError(state, data) {
      state.error = data;
    },
    storeUser(state, data) {
      state.user = data;
      state.trustee = data.trustee;
    },
    storeApplication(state, data) {
      state.application = data;
    },
    logout(state) {
      state.user = null;
      state.details = null;
      state.classified = null;
    },
    storeDetails(state, data) {
      state.details = data;
    },
    addDeals(state, data) {
      data.forEach((d) => state.deals.push(d));
    },
    addDealToHistory(state, data) {
      data.forEach((d) => state.dealHistory.push(d));
    },
    removeDeals(state, data) {
      state.deals = state.deals.filter((d) => !data.includes(d.id));
    },
    acceptDeal(state, data) {
      state.deals.forEach((d) => {
        if (d.id == data.id) {
          d.accepted = true;
          state.user.tide += d.value;
        }
      });
    },
    autoAcceptDeal(state) {
      var done = false;
      state.deals.forEach((d) => {
        if (done) return;
        d.accepted = true;
        state.user.tide += d.value;
        done = true;
      });
    },
    updateTide(state, data) {
      state.user.tide = data;
    },
    updateRoute(state, data) {
      state.route = data;
    },

    updateTrustee(state, data) {
      state.user.trustee = data;
    },
  },
  getters: {
    loading: (state) => state.loading,
    user: (state) => state.user,
    application: (state) => state.application,
    details: (state) => state.details,
    error: (state) => state.error,
    network: (state) => state.network,
    deals: (state) => state.deals,
    dealHistory: (state) => state.dealHistory,
    random: (state) => state.random,
    route: (state) => state.route,
  },
  actions: {
    getDeals(context, add) {
      try {
        // Remove accepted deals and add to history
        const dealsToRemove = context.getters.deals.filter((d) => d.accepted);
        context.commit("addDealToHistory", dealsToRemove);
        context.commit(
          "removeDeals",
          dealsToRemove.map((d) => d.id)
        );

        // Add new deals
        if (context.getters.deals.length < 3 && add) {
          context.commit("addDeals", [config.getNextDeal()]);
        }
      } catch (thrownError) {
        console.log(thrownError);
      }
    },
  },
});
