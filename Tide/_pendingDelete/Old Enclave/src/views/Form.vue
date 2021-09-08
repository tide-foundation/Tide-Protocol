<template>
  <div>
    <h2>Update your details</h2>
    <form id="update-form" @submit.prevent="update">
      <div class="form-field" v-for="(field, index) in fields" :key="index">
        <label class="form-label" for="">{{ field.field }}</label>
        <input class="mt-10 form-input" type="text" v-model="field.value" required :placeholder="field.field" />
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
      encrypted: true,
      formData: this.$store.getters.formData,
      account: this.$store.getters.account,
      key: this.$store.getters.encryptionKey,
    };
  },
  created() {
    this.encrypted = this.formData.type == "modify";

    this.fields = MetaField.fromModel(this.formData.data, this.encrypted, this.formData.validation, this.formData.classification, this.formData.tags);

    if (this.encrypted) {
      for (const field of this.fields) {
        field.decrypt(this.key);
      }
    }
  },
  methods: {
    async update() {
      this.$loading(true, "Encrypting your data and returning it to the vendor");
      setTimeout(async () => {
        for (const field of this.fields) {
          field.encrypt(this.key);
        }

        await this.$store.dispatch("postData", MetaField.buildModel(this.fields));

        this.$store.dispatch("closeWindow");
      }, 50);

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
    top: 10px;
    left: 5px;
    font-size: 12px;
    color: gray;
  }
}
</style>
