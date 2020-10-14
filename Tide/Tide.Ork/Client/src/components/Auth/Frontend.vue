<template>
    <div id="frontend">
        <div v-if="$store.getters.loggedIn">
            <div id="nav">
                <router-link to="/">Home</router-link>|
                <router-link to="/protected">Protected</router-link>
            </div>
            <router-view />
        </div>
        <Auth v-else></Auth>
    </div>
</template>


<script>
import Auth from "./Auth.vue";
export default {
    components: {
        Auth,
    },
};
</script>

<style lang="scss">
</style>
