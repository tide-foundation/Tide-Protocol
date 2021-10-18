<template>
  <div id="form">
    <div class="row">
      <div class="col-12">
        <h2>Update your details</h2>
      </div>
    </div>

    <form id="update-form" class="f-c" @submit.prevent="update">
      <div id="form-padding" class="invisible">.</div>
      <div class="row">
        <div class="col-12 col-md-4" v-for="(field, index) in fields">
          <tide-input :key="index" :id="field.field" v-model="field.value">{{ field.friendlyName }}</tide-input>
        </div>
      </div>

      <div class="row mb-30">
        <div class="col-12 col-md-4">
          <button class="mt-20 full-width" type="submit">UPDATE DETAILS</button>
        </div>
      </div>
    </form>
  </div>
</template>

<script lang="ts">
import Base from "@/assets/ts/Base";
// @ts-ignore
import MetaField from "@/assets/ts/MetaField";
// @ts-ignore
import { C25519Key } from "../../../../Tide.Js/src/export/TideAuthentication";

export default class Form extends Base {
  fields: any[] = [];
  encrypted: boolean = true;
  formData: any;
  key: C25519Key;

  mounted() {
    this.key = C25519Key.fromString(this.mainStore.getState.account!.encryptionKey);
    this.formData = this.mainStore.getState.config.formData;

    this.encrypted = this.formData.type == "modify";

    this.fields = MetaField.fromModel(this.formData.data, this.encrypted, this.formData.validation, this.formData.classification, this.formData.tags);

    for (const field of this.fields) {
      const splitCamel = field.field.replace(/([a-z])([A-Z])/g, "$1 $2") as string;
      field.friendlyName = `${splitCamel[0].toUpperCase()}${splitCamel.substring(1)}`;

      if (this.encrypted) field.decrypt(this.key);
    }
  }

  async update() {
    setTimeout(async () => {
      for (const field of this.fields) {
        field.encrypt(this.key);
      }

      this.mainStore.returnFormData(this.fields);
    }, 50);
  }
}
</script>

<style lang="scss" scoped>
#form {
  width: 100%;
  max-width: 1150px;
  // min-height: 500px;

  h2 {
    margin-left: 20px;
    font-size: 1.4rem;
  }

  #update-form {
    width: 100%;

    overflow-y: auto;

    #form-padding {
      display: block;
    }
    .row {
      width: 100%;
      .col-4 {
        button {
          width: 100%;
        }
      }
    }
  }
}
</style>
