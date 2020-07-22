<template>
  <div id="sidebar">
    <div id="logo-box">
      <img src="../../assets/logo.svg" alt="Tide Logo" />
    </div>

    <Section v-for="(section, sectionIndex) in sections" :section="section" :last="section.l" :key="sectionIndex" />
  </div>
</template>

<script>
import Section from "@/components/docs/SidebarSection.vue";
export default {
  name: "Sidebar",
  components: {
    Section,
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
              e: true,
              u: "about",
            },
          ],
        },
        {
          n: "INTEGRATION",
          l: true,
          categories: [
            {
              n: "Before You Start",
              e: false,
              items: [
                { n: "Tech Summary", u: "tech-summary", e: false },
                { n: "High-Level Overview", u: "high-level", e: false },
                { n: "How It Works", u: "how-it-works", e: false },
              ],
            },
            {
              n: "Tide Client",
              e: false,
              items: [
                { n: "Installation", u: "not-created", e: false },
                { n: "Getting Started", u: "not-created", e: false },
                { n: "Accounts", u: "not-created", e: false },
                { n: "Encryption", u: "not-created", e: false },
                { n: "User Settings", u: "not-created", e: false },
              ],
            },
            {
              n: "Tide Server",
              e: false,
              items: [
                { n: "Installation", u: "not-created", e: false },
                { n: "Getting Started", u: "not-created", e: false },
                { n: "Vendor Settings", u: "not-created", e: false },
                { n: "Data & Serialization", u: "not-created", e: false },
              ],
            },
          ],
        },
        {
          n: "API",
          l: false,
          categories: [
            {
              n: "Tide JS",
              e: false,
              u: "tide-js",
            },
            {
              n: "Tide C#",
              e: false,
              u: "tide-csharp",
            },
          ],
        },
      ],
      firstNode: null,
    };
  },
  created() {
    // Assign ids
    var id = 0;
    this.sections.forEach((section) => {
      section.categories.forEach((category) => {
        category.id = id++;
        if (category.items != null) {
          category.items.forEach((item) => {
            item.id = id++;
            if (this.firstNode == null) this.firstNode = item;
          });
        } else {
          if (this.firstNode == null) this.firstNode = category;
        }
      });
    });

    this.$bus.$on("doc-route", (data) => {
      this.$store.commit("updateSelected", data.id);

      localStorage.selected = JSON.stringify({ id: data.id, u: data.u });

      return this.$router.push(`/docs/${data.u}`).catch(() => {});
    });

    this.firstRoute();
  },
  methods: {
    firstRoute() {
      var data;
      try {
        if (localStorage.selected) {
          var json = JSON.parse(localStorage.selected);
          if (json.id == null || json.u == null) throw new Error();
          data = json;
        }
      } catch (error) {
        data = this.firstNode;
      }

      this.$bus.$emit("doc-route", data);
    },
  },
};
</script>

<style scoped lang="scss">
#sidebar {
  width: 100%;
  background: #fafafa;
  height: 100vh;
  position: static;

  #logo-box {
    height: 65px;
    padding-left: 60px;
    display: flex;
    justify-content: flex-start;
    align-items: center;
    margin-bottom: 20px;
    img {
      height: 40px;
    }
  }

  ul {
    margin: 0px;
    padding: 0px;
    list-style-type: none;
    li {
      padding-left: 50px;
    }
  }

  ul {
    padding: 0px !important;
  }
}
</style>
