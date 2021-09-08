<template>
  <div id="form">
    <div class="row">
      <div class="col-12">
        <h2>Update your details</h2>
      </div>
    </div>

    <form id="update-form" class="f-c" @submit.prevent="update">
      <div id="form-padding">.</div>
      <div class="row">
        <div class="col-12 col-md-4" v-for="(field, index) in fields">
          <tide-input :key="index" :id="field.field" v-model="field.value">{{ field.friendlyName }}</tide-input>
        </div>
      </div>

      <div class="row">
        <div class="col-4">
          <button class="mt-20" type="submit">UPDATE DETAILS</button>
        </div>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import mainStore from "@/store/mainStore";
import TideInput from "@/components/Tide-Input.vue";
import { ref, computed, onMounted } from "vue";
// @ts-ignore
import MetaField from "@/assets/ts/MetaField";

var fields = ref<any>([]);
var encrypted = ref(true);
var formData = ref(mainStore.getState.config.formData);

onMounted(() => {
  encrypted.value = formData.value.type == "modify";

  fields.value = MetaField.fromModel(
    formData.value.data,
    encrypted.value,
    formData.value.validation,
    formData.value.classification,
    formData.value.tags
  );

  for (const field of fields.value) {
    const splitCamel = field.field.replace(/([a-z])([A-Z])/g, "$1 $2") as string;
    field.friendlyName = `${splitCamel[0].toUpperCase()}${splitCamel.substring(1)}`;

    if (encrypted.value) field.decrypt(mainStore.getState.account!.encryptionKey);
  }
});

const update = async () => {
  setTimeout(async () => {
    for (const field of fields.value) {
      field.encrypt(mainStore.getState.account!.encryptionKey);
    }

    mainStore.returnFormData(fields.value);
  }, 50);
};
</script>

<style lang="scss" scoped>
#form {
  width: 100%;

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
