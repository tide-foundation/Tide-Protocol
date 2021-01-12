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
export default {
  data() {
    return {
      fields: [],
    };
  },
  created() {
    this.fields = MetaField.fromModel(this.$store.getters.formData.data, false);
  },
  methods: {
    async update() {
      for (const field of this.fields) {
        //  field.encrypt(this.key);
      }

      await this.$store.dispatch("postData", this.fields);
      // if (this.$store.getters.formData.closeAfter) this.$store.dispatch("closeWindow");
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
