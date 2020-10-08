import Vue from "vue";
import Vuex from "vuex";
import router from "../router";

Vue.use(Vuex);

var orks = [];
for (let i = 0; i < 3; i++) {
  orks.push({
    id: i,
    // url: `https://dork${i + 1}.azurewebsites.net`,
    // url: `https://ork-${i}.azurewebsites.net`,
    url: `http://localhost:500${i + 1}`,
    cmk: false,
    cvk: false,
  });
}

//var vendorUrl = "https://tidevendor.azurewebsites.net";
var vendorUrl = "http://127.0.0.1:6001";

export default new Vuex.Store({
  state: {
    mode: "Frontend",
    user: null,
    orks: orks,
    vendorUrl: vendorUrl,
    loading: {
      active: false,
      text: "Loading...",
    },
  },
  mutations: {
    UPDATE_MODE(state, newMode) {
      state.mode = newMode;
    },
    UPDATE_LOADING(state, data) {
      state.loading = data;
    },
    SET_USER(state, user) {
      state.user = user;
      router.push(user != null ? "/protected" : "/");
    },
  },
  actions: {
    getDeals(context, user) {},
  },
  modules: {},
  getters: {
    mode: (state) => state.mode,
    user: (state) => state.user,
    loading: (state) => state.loading,
    loggedIn: (state) => state.user != null,
    vendorUrl: (state) => state.vendorUrl,
    orks: (state) => state.orks,
    tempOrksToUse: (state) => state.orks.filter((o) => o.id < 3).map((o) => o.url),
  },
});
