import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    selected: 0,
  },
  mutations: {
    updateSelected(state, data) {
      state.selected = data;
    },
  },
  actions: {},
  modules: {},
  getters: {
    selected: (state) => state.selected,
  },
});
