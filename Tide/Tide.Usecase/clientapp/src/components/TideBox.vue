<template>
  <section
    id="tide-box"
    v-if="show"
  >
    <div
      @click="toggle"
      :class="{'selected shadow' : $store.getters.tideEngaged,'processing':$store.getters.tideProcessing,'button-glow':!$store.getters.tideEngaged && !$store.getters.clickedTide}"
      id="btn-container"
    >
      <img
        :class="{'shake':!$store.getters.tideEngaged && !$store.getters.clickedTide}"
        src="../assets/img/logo.svg"
        alt="side button"
      >
    </div>
  </section>
</template>

<script>
export default {
  data() {
    return {
      show: false
    }
  },
  created() {
    this.checkShow(this.$router.currentRoute.name);
    this.$bus.$on('route-change', (r) => {
      this.checkShow(r);
    })
  },
  methods: {
    toggle() {
      if (this.$store.getters.user == null) {
        this.$bus.$emit('showLoginModal', true)
        this.$bus.$emit('show-message', 'Please login or register to continue')
        this.$store.commit('updateRoute', { action: 'event', value: 'init' });
        return;
      }
      this.$store.commit('updateClickedTide', true);
      this.$bus.$emit('toggle-tide');
    },
    checkShow(r) {
      if (r == 'apply' || r == 'profile') {
        this.show = true;
      } else {
        this.show = false;
      }
    }
  }
}
</script>

<style lang="scss">
#btn-container {
  width: 60px;
  height: 60px;
  background: grey;
  border-radius: 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;

  img {
    width: 30px;
  }
}

.selected {
  background: #03a9f4 !important;
}

.processing {
  transition: all 0.3s ease-in-out;
  pointer-events: none;
  opacity: 0.3;
}

.shadow {
  box-shadow: 0px 6px 32px -1px rgba(0, 0, 0, 0.75);
}

#btn-container:hover {
  transition: all 0.3s ease-in-out;

  background: #03a9f4;
  img {
    transform: rotate(360deg);
    transition: all 0.3s ease-in-out;
  }
}

#tide-box {
  position: fixed;
  top: 150px;
  right: 15px;
  z-index: 300;
}

.button-glow {
  animation: glowing 2000ms infinite;
}

.shake {
  animation: shake 2000ms infinite;
}

@keyframes glowing {
  0% {
    box-shadow: 0 0 -10px #c36a00;
    background: grey;
  }
  40% {
    box-shadow: 0 0 30px #c36a00;
    background: orange;
  }
  60% {
    box-shadow: 0 0 30px #c36a00;
    background: orange;
  }
  100% {
    box-shadow: 0 0 -10px #c36a00;
    background: grey;
  }
}

@keyframes shake {
  0% {
    transform: rotate(0deg);
  }
  30% {
    transform: rotate(0deg);
  }
  32% {
    transform: rotate(-10deg);
  }
  34% {
    transform: rotate(10deg);
  }
  36% {
    transform: rotate(-10deg);
  }
  38% {
    transform: rotate(10deg);
  }
  40% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(0deg);
  }
}
</style>
