<template>
  <div id="sidebar">
    <div id="logo-box">
      <img src="../../assets/logo.svg" alt="Tide Logo" />
    </div>

    <Section v-for="(section, sectionIndex) in sections" :section="section" :last="section.l" :key="sectionIndex" />
    >
  </div>
</template>

<script>
import Section from "@/components/docs/SidebarSection.vue";
export default {
  name: "Sidebar",
  components: {
    Section,
  },
  props: {
    sections: Array,
  },
  data: function() {
    return {};
  },
  created() {
    this.$bus.$on("item-clicked", (id) => {
      this.selected(id);
    });

    // Assign unique ids
    var id = 0;
    this.sections.forEach((section) => {
      section.categories.forEach((category) => {
        if (category.items != null) {
          category.items.forEach((item) => {
            item.id = id++;
            if (item.e) this.selected(item.id);
          });
        }
      });
    });
  },
  methods: {
    selected(id) {
      this.sections.forEach((section) => {
        section.categories.forEach((category) => {
          if (category.items != null) {
            category.items.forEach((item) => {
              if (item.id == id) {
                item.e = true;

                this.$router.push(`/docs/${item.u}`);
              } else {
                item.e = false;
              }
            });
          }
        });
      });
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
