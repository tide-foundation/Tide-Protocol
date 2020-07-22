<template>
  <div id="docs">
    <div id="page-sidebar">
      <Sidebar />
    </div>

    <div id="page-content">
      <div id="content-container">
        <Topbar />
        <Content />
      </div>
    </div>
  </div>
</template>

<script>
import Sidebar from "@/components/docs/Sidebar.vue";
import Topbar from "@/components/docs/Topbar.vue";
import Content from "@/components/docs/Content.vue";
export default {
  components: {
    Sidebar,
    Topbar,
    Content,
  },
};
</script>

<style lang="scss">
#docs {
  width: 100%;
  display: flex;
  flex-direction: row;

  #page-sidebar {
    width: 320px;
    position: fixed;
  }

  #page-content {
    display: flex;
    flex-direction: column;
    width: calc(100% - 320px);
    margin-left: 320px;

    #content-container {
      max-width: 1000px;
    }
  }
}
</style>
