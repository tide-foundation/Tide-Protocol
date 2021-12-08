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
import Num64 from "../../../../Tide.Js/src/Num64";
// @ts-ignore
import { C25519Key } from "../../../../Tide.Js/src/export/TideAuthentication";

interface Field {
  Name: string;
  Value: string;
}

interface DynamicForm {
  [key: string]: any;
}

export default class Form extends Base {
  fields: any[] = [];
  encrypted: boolean = true;
  key: C25519Key;

  mounted() {
    this.key = C25519Key.fromString(this.mainStore.getState.account!.encryptionKey);
    const formData = this.mainStore.getState.config.formData;
    var raw = JSON.parse(JSON.stringify(formData));
    var structure = raw.structure as any[];
    var userData = raw.data as any[];
    if (this.isEmpty(userData)) userData = [];

    structure.forEach((field: Field) => {
      // Get the current field value, or create an empty string
      var v = userData.find((d: any) => d.key == field.Name);
      var val = v != null && v.value != "" ? v.value : "";

      // Generate the form field and decrypt it if it's not new
      var metaField = MetaField.fromText(field.Name, val, val != "");
      if (val != "") metaField.decrypt(this.key);

      // Set the tag, this will be dynamic in the future
      metaField.tag = Num64.seed("__vendor__");

      // Create a friendly name for the field by splitting camelCase
      const splitCamel = metaField.field.replace(/([a-z])([A-Z])/g, "$1 $2") as string;
      metaField.friendlyName = `${splitCamel[0].toUpperCase()}${splitCamel.substring(1)}`;

      this.fields.push(metaField);
    });
  }

  isEmpty(obj: object) {
    for (var prop in obj) {
      if (Object.prototype.hasOwnProperty.call(obj, prop)) {
        return false;
      }
    }

    return JSON.stringify(obj) === JSON.stringify({});
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
