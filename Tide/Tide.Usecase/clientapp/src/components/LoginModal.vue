<template>
  <div id="login-overlay" v-if="show">
    <div class="modal-dialog">
      <div class="login-section modal-section">
        <div class="form-wrapper">
          <div class="form-heading clearfix">
            <button
              type="button"
              class="close close-modal-dialog "
              @click="show = false"
            >
              <i class="fa fa-times fa-lg"></i>
            </button>
            <img
              class="header-logo"
              src="../assets/img/FuturePlacesLogo.png"
              alt=""
            />
            <transitionBox>
              <login v-if="!registerMode" />
              <register v-if="registerMode" />
            </transitionBox>
            <img
              class="tide-logo"
              src="../assets/img/privacy-by-tide.png"
              title="Your sensitive data is encrypted via Tide, and you have the only key. Putting you in control of your data."
              alt="Your sensitive data is encrypted via Tide, and you have the only key. Putting you in control of your data."
            />
          </div>
        </div>
      </div>
    </div>
    <!-- .modal-dialog -->
  </div>
</template>

<script>
import transitionBox from '../components/TransitionBox.vue'
import Register from '../components/Register.vue'
import Login from '../components/Login.vue'
export default {
  components: {
    transitionBox,
    Register,
    Login
  },
  data() {
    return {
      registerMode: true,
      show: false
    }
  },
  created() {
    this.$bus.$on('changeRegisterMode', mode => this.registerMode = mode);
    this.$bus.$on('showLoginModal', show => {
      if (show && this.$store.getters.user != null) {
        return location.reload();
      }
      this.show = show
    });
  },
}
</script>

<style scoped>
#login-overlay {
  text-align: center;
  z-index: 800;
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
  background-color: rgba(0, 0, 0, 0.8);
}

.modal-dialog {
  text-align: left;
  z-index: 850;
  height: 500px;
}

.header-logo {
  display: block;
  margin: 0 auto 20px auto;
  width: 160px !important;
}

.tide-logo {
  display: block;
  margin: 30px auto 0 auto;
}
</style>
