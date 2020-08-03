<template>
  <section class="tide-input">
    <p :class="{ 'white-text': !lockedText }">
      {{ label }}
      <span class="classification" v-if="classification != null"
        >({{ classification }})</span
      >
    </p>
    <div class="input-content">
      <input
        :markup="markup"
        :name="guid"
        :value="value"
        :type="type"
        :list="list"
        autocomplete="new-password"
        class="effect tide-input-main"
        @input="$emit('input', $event.target.value)"
      />
      <span class="focus-border">
        <i></i>
      </span>
    </div>
  </section>
</template>

<script>
export default {
  props: [
    "value",
    "label",
    "type",
    "required",
    "disabled",
    "markup",
    "list",
    "classification",
    "tidify"
  ],
  data() {
    return {
      content: this.value,
      guid: (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1),
      locked: true,
      lockedText: true,
      engaged: false
    };
  },
  created() {
    this.$bus.$on("update-lock-start", l => {
      if (l.key == this.guid) this.lockedText = l.val;
    });

    this.$bus.$on("update-lock-end", l => {
      if (l.key == this.guid) {
        this.locked = l.val;
        this.src = l.val
          ? "../assets/img/tide-lock.svg"
          : "../assets/img/tide-unlock.svg";
      }
    });

    this.$bus.$on("engage-input", l => {
      if (l.key == this.guid) this.engaged = l.val;
    });
  },
  methods: {}
};
</script>

<style lang="scss" scoped>
$primary-color: orange;

.white-text {
  transition: all 0.3s ease;
  color: white;
}

.tide-input {
  margin-bottom: 50px;
  text-align: left;
  z-index: 250;
}

p {
  margin-bottom: 8px;
}

:focus {
  outline: none;
}

.input-content {
  position: relative;
}

input {
  color: black;
  width: 100%;
  box-sizing: border-box;
  letter-spacing: 1px;
  height: 45px;
  transition: 1s ease-in-out;
  background-color: #fcfcfc !important;
}

input:disabled {
  border: #333742 1px solid;
}

input::placeholder {
  color: #e4e5ea;
}

.effect {
  border: 1px solid #ccc;
  padding: 7px 14px;
  transition: 0.4s;
  background: transparent;
}

.effect ~ .focus-border:before,
.effect ~ .focus-border:after {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  width: 0;
  height: 2px;
  background-color: $primary-color;
  transition: 0.3s;
}

.effect ~ .focus-border:after {
  top: auto;
  bottom: 0;
  left: auto;
  right: 0;
}

.effect ~ .focus-border i:before,
.effect ~ .focus-border i:after {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  width: 2px;
  height: 0;
  background-color: $primary-color;
  transition: 0.4s;
}

.effect ~ .focus-border i:after {
  left: auto;
  right: 0;
  top: auto;
  bottom: 0;
}

.no-events {
  pointer-events: none;
  opacity: 0.5;
}

.effect:focus ~ .focus-border:before,
.effect:focus ~ .focus-border:after {
  width: 100%;
  transition: 0.3s;
}

.effect:focus ~ .focus-border i:before,
.effect:focus ~ .focus-border i:after {
  height: 100%;
  transition: 0.4s;
}

input[type="date"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  display: none;
}

input[type="date"]::-webkit-calendar-picker-indicator {
  color: white;
  cursor: pointer;
}

input[type="date"]::-webkit-calendar-picker-indicator:hover {
  color: #252831;
  background-color: transparent;
}

.locked {
  pointer-events: none;
}

.tide-engaged {
  transition: all 0.3s ease;
  border: 2px solid #26ade4 !important;
  pointer-events: auto;
  box-shadow: 0px 0px 10px 0px rgba(0, 4, 255, 0.75);
}
</style>
