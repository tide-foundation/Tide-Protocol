<template>
  <div id="vendor-backend">
    <div id="button-bar" v-if="selectedUser != null">
      <button @click="selectedUser = null">Back</button>
      <button @click="decryptUser">Decrypt</button>
    </div>
    <table v-if="selectedUser == null">
      <tr class="r-h">
        <th>UserId</th>
      </tr>

      <tr v-for="user in users.filter(u=>u.rentalApplications != null && u.rentalApplications.length > 0)" :key="user.id" @click="selectedUser = user">
        <td>{{ user.id }}</td>
      </tr>
    </table>

    <table v-if="selectedUser != null">
      <tr class="r-h" v-for="(value, propertyName)  in selectedUser.rentalApplications[0]" :key="propertyName" @click="decryptField(propertyName,value)">
        <th>{{propertyName}}</th>
        <td>{{value}}</td>
        <!-- <td>{{selectedUser.rentalApplications[0][property]}}</td> -->
      </tr>
      <!-- <tr v-for="user in users" :key="user.id" @click="selectedUser = user">
        <td>{{ user.id }}</td>
      </tr>-->
    </table>
  </div>
</template>

<script>
export default {
    data: function() {
        return {
            users: [],
            selectedUser: null,
            data: null
        };
    },
    async created() {
        this.users = (await this.$http.get(`${this.$tide.serverUrl}/BackendTest`)).data;
    },
    methods: {
        clickedUser() {},
        async decryptUser() {
            var data = (await this.$http.get(`${this.$tide.serverUrl}/BackendTest/${this.selectedUser.rentalApplications[0].id}`)).data;
            console.log(data);
        },
        async decryptField(property, value) {
            var data = (await this.$http.post(`${this.$tide.serverUrl}/BackendTest/single/${this.selectedUser.id}`, { field: value })).data;
            this.selectedUser.rentalApplications[0][property] = data;
        }
    }
};
</script>

<style scoped lang="scss">
.r-h {
    cursor: pointer;
    &:hover {
        background: rgba(255, 255, 0, 0.473);
    }
}
</style>