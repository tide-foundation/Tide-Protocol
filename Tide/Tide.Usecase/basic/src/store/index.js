import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

var orks = [];
for (let i = 0; i < 10; i++) {
  orks.push({
    id: i,
    url: `https://ork-${i}.azurewebsites.net`,
    // url: `http://localhost:500${i + 1}`,
    cmk: false,
    cvk: false,
  });
}

const shuffled = orks.sort(() => 0.5 - Math.random());
for (let i = 0; i < 3; i++) {
  shuffled[i].cmk = true;
}
for (let i = 3; i < 7; i++) {
  shuffled[i].cvk = true;
}
var vendorUrl = "https://tidevendor.azurewebsites.net";
//var vendorUrl = "http://127.0.0.1:6001";

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
