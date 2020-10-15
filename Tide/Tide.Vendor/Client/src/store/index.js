import Vue from "vue";
import Vuex from "vuex";
import router from "../router";

Vue.use(Vuex);

var orks = [];
for (let i = 0; i < 20; i++) {
  orks.push({
    id: i,
    url: `https://pdork${i + 1}.azurewebsites.net`,
    // url: `https://dork${i + 1}.azurewebsites.net`,
    // url: `https://ork-${i}.azurewebsites.net`,
    //url: `http://localhost:500${i + 1}`,
    cmk: false,
    cvk: false,
  });
}

export default new Vuex.Store({
  state: {
    user: null,
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
      router.push(user != null ? "/protected" : "/");
    },
  },
  actions: {
    //getDeals(context, user) {},
  },
  modules: {},
  getters: {
    user: (state) => state.user,
    loading: (state) => state.loading,
    loggedIn: (state) => state.user != null,
  },
});
