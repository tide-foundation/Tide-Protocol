<template>
  <section class="settings-section">
    <div class="box" id="account">
      <h4>Account</h4>
      <div class="row">
        <div class="settings-field col-xs-6 col-sm-4">
          <label>Tide Username</label>
          <input
            class="inline"
            type="text"
            :value="$store.getters.user.username"
            disabled
          />
        </div>
        <div class="settings-field col-xs-6 col-sm-4">
          <label>Tide Account</label>
          <input
            class="inline"
            type="text"
            :value="$store.getters.user.account"
            disabled
          />
        </div>
      </div>
    </div>

    <div class="box" id="credentials">
      <h4>Credentials</h4>
      <div class="row">
        <div class="settings-field col-xs-12">
          <label>Tide Keys</label>
          <section class="split">
            <input
              class="long mr col-xs-6"
              type="text"
              :value="$store.getters.user.keys.tide.pub"
              disabled
            />
            <input
              class="long col-xs-6 ml click-reveal"
              type="text"
              :value="
                tideRevealed
                  ? $store.getters.user.keys.tide.priv
                  : 'Click to reveal'
              "
              @click="tideRevealed = true"
              readonly
            />
          </section>
        </div>
      </div>

      <div class="row">
        <div class="settings-field col-xs-12">
          <label>Future Places Keys</label>
          <section class="split">
            <input
              class="long mr col-xs-6"
              type="text"
              :value="$store.getters.user.keys.vendor.pub"
              disabled
            />
            <input
              class="long col-xs-6 ml click-reveal"
              type="text"
              :value="
                vendorRevealed
                  ? $store.getters.user.keys.vendor.priv
                  : 'Click to reveal'
              "
              @click="vendorRevealed = true"
              readonly
            />
          </section>
        </div>
      </div>
    </div>

    <div class="box" id="nodes">
      <h4>Nodes</h4>
      <div class="row">
        <div class="settings-field col-xs-6 col-sm-4">
          <input
            v-for="(node, index) in $config.orkNodes"
            :key="index"
            class="inline"
            type="text"
            :value="node"
            disabled
          />
        </div>
      </div>
    </div>
  </section>
</template>

<script>
export default {
  data() {
    return {
      vendorRevealed: false,
      tideRevealed: false
    };
  },

  methods: {}
};
</script>

<style lang="scss" scoped>
.box {
  padding: 10px;
  margin-bottom: 20px;
}

#account {
  border-top: 2px solid #01acfe;
}

#credentials {
  border-top: 2px solid #cc2a41;
}
#nodes {
  border-top: 2px solid #8fbe00;
}
.settings-field {
  label {
    display: block;
  }

  input {
    border: #d3dce2 1px solid;
    padding-left: 5px;
    height: 30px;
    margin-bottom: 20px;
  }

  .inline {
    width: 100%;
  }

  input:disabled {
    background: #e2e4e5;
  }
}

h4 {
  margin: 0 0 10px 0;
}

.long {
  width: 100%;
}

.ml {
  margin-left: 15px;
}

.mr {
  margin-right: 15px;
}

.split {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}

.click-reveal {
  background: #e2e4e5;
  pointer-events: auto !important;
  cursor: pointer;
}
</style>
