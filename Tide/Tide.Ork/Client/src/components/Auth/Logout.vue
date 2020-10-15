<template>
    <div>
        <h1>Logout</h1>
        <form @submit.prevent="logout">
            <button type="submit">LOGOUT</button>
        </form>
    </div>
</template>

<script>
export default {
    methods: {
        async logout() {
            try {
                this.$tide.logout();
                this.$parent.setUser(null);
            } catch (error) {
                this.$bus.$emit("show-status", error);
            } finally {
                this.$loading(false, "");
            }
        },
    },
};
</script>

<style lang="scss" scoped>
h1 {
    text-align: center;
}
</style>