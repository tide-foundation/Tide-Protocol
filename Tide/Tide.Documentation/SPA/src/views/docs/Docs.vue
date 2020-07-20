<template>
  <div id="docs">
    <div id="page-sidebar">
      <Sidebar :sections="sections" />
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
  data: function() {
    return {
      sections: [
        {
          n: "GENERAL",
          l: false,
          categories: [
            {
              n: "About Tide",
              e: false,
            },
          ],
        },
        {
          n: "INTEGRATION",
          l: true,
          categories: [
            {
              n: "Before You Start",
              e: true,
              items: [
                { n: "A Tide Summary", u: "summary", e: true },
                { n: "High-Level Overview", u: "high-level", e: false },
                { n: "How It Works", u: "how-it-works", e: false },
              ],
            },
            {
              n: "Tide Client",
              e: false,
              items: [
                { n: "Installation", u: "#", e: false },
                { n: "Getting Started", u: "#", e: false },
                { n: "Accounts", u: "#", e: false },
                { n: "Encryption", u: "#", e: false },
                { n: "User Settings", u: "#", e: false },
              ],
            },
            {
              n: "Tide Server",
              e: false,
              items: [
                { n: "Installation", u: "#", e: false },
                { n: "Getting Started", u: "#", e: false },
                { n: "Vendor Settings", u: "#", e: false },
                { n: "Data & Serialization", u: "#", e: false },
              ],
            },
          ],
        },
      ],
    };
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
  }

  #page-content {
    display: flex;
    flex-direction: column;
    width: calc(100% - 320px);

    #content-container {
      max-width: 1000px;
    }
  }
}
</style>
