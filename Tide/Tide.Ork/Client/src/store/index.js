import Vue from "vue";
import Vuex from "vuex";
import router from "../router";

import Tide from "../../../../Tide.Js/src/export/TideAuthentication";
import request from "superagent";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    initialized: false,
    mode: "Frontend",
    orks: [],
    vendorUrl: null,
    vendorPublic: null,
    vendorServer: null,
    homeUrl: null,
    debug: false,
    tide: null,
    loading: {
      active: false,
      text: "Loading...",
    },
    action: "Login",
    goToDashboard: false,
    account: null,
    sessionId: null,
    origin: `${window.location.protocol}//${window.location.host}`,
    formData: null,
    keepOpen: false,
    source: null,
    origin: null,
    event: null,
    encryptionKey: null,
  },
  mutations: {
    UPDATE_MODE(state, newMode) {
      state.mode = newMode;
    },
    UPDATE_LOADING(state, data) {
      state.loading = data;
    },
    UPDATE_SESSION_ID(state, sessionId) {
      state.sessionId = sessionId;
    },
    UPDATE_SOURCE(state, source) {
      state.source = source;
    },
    UPDATE_ORIGIN(state, origin) {
      state.origin = origin;
    },
    UPDATE_EVENT(state, event) {
      state.event = event;
    },
    SET_ENCRYPTION_KEY(state, key) {
      state.encryptionKey = key;
    },
  },
  actions: {
    async initializeTide(context, data) {
      try {
        context.state.initialized = true;
        context.state.vendorUrl = data.vendorUrl;
        context.state.vendorPublic = data.vendorPublic;
        context.state.vendorServer = data.serverUrl;
        context.state.orks = data.orks;
        context.state.debug = data.debug;
        context.state.vendorName = data.vendorName;
        context.state.keepOpen = data.keepOpen;

        context.state.formData = data.formData;

        context.state.tide = new Tide("VendorId", data.vendorUrl, data.orks, data.vendorPublic);

        // var silentLoginAccount = context.state.tide.loginSilently();
        // if (silentLoginAccount != null) {
        //   await context.dispatch("finalizeAuthentication", silentLoginAccount);
        //   return;
        // }

        // if (!context.state.tide.validateReturnUrl(Vue.prototype.$bus.origin, data.hashedReturnUrl)) {
        //   return Vue.prototype.$bus.source.postMessage({ type: "tide-failed", data: { error: "Failed to validate returnUrl" } }, Vue.prototype.$bus.origin);
        // }

        if (data.formData != null) router.push("/auth?mode=form");
        else router.push("/auth");
      } catch (error) {
        this.$bus.$emit("initError", { msg: "Failed Initializing", error });
      }
    },

    async checkForValidUsername(context, username) {
      return context.state.tide.checkForValidUsername(username);
    },
    async changeOrkWindow(context, newOrk) {
      const data = { newOrk };
      Vue.prototype.$bus.source.postMessage({ type: "tide-change-ork", data }, Vue.prototype.$bus.origin);
    },
    async registerAccount(context, user) {
      context.state.action = "Register";
      context.state.goToDashboard = user.goToDashboard || context.state.keepOpen;
      const serverTime = (await request.get(`${context.state.vendorServer}/tide-utility/servertime`)).text;

      const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      var isEmail = re.test(String(user.username).toLowerCase());

      context.state.account = await context.state.tide.registerJwt(
        user.username,
        user.password,
        isEmail ? user.username : "noemail@noemail.com",
        context.state.orks,
        serverTime,
        context.state.orks.length
      );

      console.log("Account:", context.state.account);

      return context.state.account;
    },

    async loginAccount(context, user) {
      context.state.action = "Login";
      context.state.goToDashboard = user.goToDashboard || context.state.keepOpen;
      const serverTime = (await request.get(`${context.state.vendorServer}/tide-utility/servertime`)).text;
      context.state.account = await context.state.tide.loginJwt(user.username, user.password, serverTime);
      return context.state.account;
    },
    async sendRecoverEmails(context, user) {
      await context.state.tide.recover(user.username, context.getters.orks);
    },
    async reconstructAccount(context, data) {
      await context.state.tide.reconstruct(data.username, data.shares, data.newPass, context.getters.orks);
    },
    async changePassword(context, user) {
      await context.state.tide.changePassword(user.password, user.newPassword, context.getters.orks);
    },
    async finalizeAuthentication(context, data) {
      context.commit("SET_ENCRYPTION_KEY", data.encryptionKey);
      data.encryptionKey = null;

      data.vuid = data.vuid.toString();
      data.action = context.state.action;
      data.autoClose = !context.state.goToDashboard && !context.state.keepOpen;
      data.action = Vue.prototype.$bus.source.postMessage({ type: "tide-authenticated", data }, Vue.prototype.$bus.origin);

      if (context.state.formData) router.push("/form");
      else context.dispatch("closeWindow");
    },
    async postData(context, data) {
      Vue.prototype.$bus.source.postMessage({ type: "tide-form", data }, Vue.prototype.$bus.origin);
    },
    closeWindow(context) {
      Vue.prototype.$bus.source.postMessage({ type: "tide-close" }, Vue.prototype.$bus.origin);
    },
  },
  modules: {},
  getters: {
    mode: (state) => state.mode,
    loading: (state) => state.loading,
    vendorUrl: (state) => state.vendorUrl,
    orks: (state) => state.orks,
    username: (state) => state.username,
    isInitialized: (state) => state.initialized,
    isLoggedIn: (state) => true,
    account: (state) => state.account,
    debug: (state) => state.debug,
    qrData: (state) => `1|${state.vendorName}|${state.vendorServer}|${state.origin}|${state.vendorPublic}|${state.sessionId}`,
    sessionId: (state) => state.sessionId,
    // origin: (state) => state.origin,
    formData: (state) => state.formData,
    source: (state) => state.source,
    origin: (state) => state.origin,
    encryptionKey: (state) => state.encryptionKey,
  },
});
