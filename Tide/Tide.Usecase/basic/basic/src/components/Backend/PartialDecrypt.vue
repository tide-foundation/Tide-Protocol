<template>
  <div id="partial-decrypt">
    <form @submit.prevent="getData(true)">
      <div class="form-group">
        <label for="field1">Field 1</label>
        <input type="text" id="field1" v-model="userData.field1" readonly />
      </div>
      <div class="form-group">
        <label for="field2">Field 2</label>
        <input type="text" id="field2" v-model="userData.field2" readonly />
      </div>
      <div class="form-group">
        <button type="submit" :disabled="fetching || decrypted">DECRYPT</button>
      </div>
    </form>
  </div>
</template>

<script>
import request from "superagent";
import { mapGetters } from "vuex";
export default {
    computed: {
        ...mapGetters(["user", "vendorUrl"]),
    },
    data() {
        return {
            fetching: false,
            decrypted: false,
            userData: {
                field1: "",
                field2: "",
            },
        };
    },
    created() {
        this.getData(false);
    },
    methods: {
        async getData(decrypted) {
            try {
                this.fetching = true;
                var response = await request.get(`${this.vendorUrl}/BackendTest/${this.user.vuid.toString()}/${decrypted}`);
                this.userData = response.body;
                this.decrypted = decrypted;
            } catch (error) {
            } finally {
                this.fetching = false;
            }
        },
    },
};
</script>

<style lang="scss" scoped>
#partial-decrypt {
}
</style>