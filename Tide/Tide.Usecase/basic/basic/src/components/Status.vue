<template>
  <div id="status">{{status}}</div>
</template>

<script>
export default {
    data() {
        return {
            status: "",
        };
    },
    created() {
        this.$bus.$on("show-status", (s) => {
            console.log(s);
            this.status = s;
            setTimeout(async () => {
                this.status = "";
            }, 5000);
        });
    },
};
</script>

<style>
#status {
    color: red;
    position: absolute;
    top: 10px;
    right: 10px;
}
</style>