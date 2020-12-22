<template>
  <div>
    <h2>Update your details</h2>
    <form id="update-form" @submit.prevent="update">
      <div class="form-field" v-for="(field, index) in fields" :key="index">
        <label for="">{{ field.field }}</label>
        <input class="mt-10" type="text" v-model="field.value" :placeholder="field.field" />
      </div>

      <button class="mt-20" type="submit">UPDATE DETAILS</button>
    </form>
  </div>
</template>

<script>
import MetaField from "../assets/js/MetaField";
import C25519Key from "cryptide/src/c25519Key";
export default {
  data() {
    return {
      fields: [],
      key: C25519Key.generate(),
    };
  },
  created() {
    this.fields = MetaField.fromModel(this.$store.getters.formData, false);
  },
  methods: {
    update() {
      for (const field of this.fields) {
        field.encrypt(this.key);
      }
    },
  },
};
</script>

<style lang="scss" scoped>
.form-field {
  position: relative;
  label {
    position: absolute;
    top: 7px;
    left: 5px;
    font-size: 12px;
    color: gray;
  }
}
</style>
