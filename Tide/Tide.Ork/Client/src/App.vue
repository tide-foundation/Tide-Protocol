<template>
    <div id="app">
        <Status></Status>
        <Loading></Loading>
        <div id="nav">
            <div v-for="(route, index) in routes" :key="index">
                <router-link :class="{ active: $route.path == route.p }" :to="route.p">{{ route.n }}</router-link>
            </div>
        </div>
        <router-view />
    </div>
</template>


<script>
import Loading from "@/components/Loading.vue";
import Status from "@/components/Status.vue";

export default {
    components: {
        Loading,
        Status,
    },
    data() {
        return {
            routes: [
                { n: "Home", p: "/" },
                { n: "Protected", p: "/protected" },
                { n: "Authenticate", p: "/auth" },
            ],
        };
    },
};
</script>

<style lang="scss">
body,
html {
    width: 100%;
    padding: 0;
    margin: 0;
}

#app {
    width: 100%;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;

    #nav {
        display: flex;
        justify-content: center;
        align-items: center;
        position: absolute;
        top: 10px;

        a {
            color: black;
            text-decoration: none;
            margin: 5px;
            &.active {
                color: orange;
            }
        }
    }
}
</style>
