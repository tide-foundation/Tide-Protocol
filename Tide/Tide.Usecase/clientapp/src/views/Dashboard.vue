<template>
  <div id="dashboard">
    <Sidebar></Sidebar>
    <Rules></Rules>
  </div>
</template>

<script>
import Sidebar from "../components/Dashboard/Sidebar.vue";
import Rules from "../components/Dashboard/Rules/Rules.vue";
export default {
    components: { Sidebar, Rules }
};
</script>

<style lang="scss" scoped>
#dashboard {
    width: 100%;
    max-width: 1140px;
    margin-top: 20px !important;
    margin: 0 auto;
    display: flex;
    flex-direction: row;
    border: 1px solid rgb(223, 223, 223);
    background-color: white;
}
</style>