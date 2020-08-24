import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

var orks = [];
for (let i = 0; i < 10; i++) {
  orks.push({
    id: i,
    url: `https://ork-${i}.azurewebsites.net`,
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

export default new Vuex.Store({
  state: {
    user: null,
    orks: orks,
    loading: {
      active: false,
      text: "Loading...",
    },
  },
  mutations: {
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
    loading: (state) => state.loading,
    loggedIn: (state) => state.user != null,
    orks: (state) => state.orks,
    tempOrksToUse: (state) => state.orks.filter((o) => o.id < 3).map((o) => o.url),
  },
});
