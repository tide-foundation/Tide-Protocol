<template>
  <div class="section">
    <h4>{{ section.n }}</h4>
    <div class="category" :class="{ expanded: category.e && category.items != null }" v-for="(category, categoryIndex) in section.categories" :key="categoryIndex">
      <div class="c-title" @click="clicked(category, null, true)">
        <div class="c-name" :class="{ selected: $store.getters.selected == category.id }">{{ category.n }}</div>
        <div class="c-chevron"><i v-if="category.items != null" class="fas" :class="[category.e ? 'fa-chevron-up' : 'fa-chevron-down']"></i></div>
      </div>

      <div class="c-content">
        <div class="c-item" @click="clicked(category, item, false)" :class="{ enabled: $store.getters.selected == item.id }" v-for="(item, itemIndex) in category.items" :key="itemIndex">
          <div class="i-bar"><div class="b-overlay"></div></div>
          <div class="i name">{{ item.n }}</div>
        </div>
      </div>
    </div>
    <div class="section-divider" v-if="!last"></div>
  </div>
</template>

<script>
export default {
  name: "SidebarSection",
  props: ["section", "last"],
  methods: {
    clicked(category, item, isCat) {
      if (isCat) {
        // If the category has items it can never be selected, so just toggle expand
        if (category.items != null) return (category.e = !category.e);
        // Else this is a lone category, so route
        return this.select(category.id, category.u);
      }
      // Route straight to item
      return this.select(item.id, item.u);
    },
    select(id, u) {
      this.$bus.$emit("doc-route", { id, u });
    },
  },
};
</script>

<style lang="scss" scoped>
.section {
  h4 {
    padding-left: 60px;
    color: #4c5b70;
  }
  .category {
    padding-left: 60px;

    height: 40px;
    transition: 0.3s;
    user-select: none;

    .c-title {
      width: 100%;
      cursor: pointer;
      display: flex;
      flex-direction: row;
      align-items: center;
      justify-content: space-between;

      .c-chevron {
        font-size: 8px;
        width: 50px;
      }

      &:hover {
        .c-name {
          color: #4bb5db;
        }
      }
      .c-name {
        &.selected {
          color: #4bb5db;
          font-weight: 500;
        }
      }
    }

    .c-content {
      margin: 15px 0;
      display: none;
      flex-direction: column;

      .c-item {
        display: flex;
        flex-direction: row;
        align-items: center;
        cursor: pointer;

        .i-bar {
          width: 2px;
          height: 40px;
          margin-right: 25px;
          background: #ced5e0;
          display: flex;
          align-items: center;
        }

        .b-overlay {
          background: #26a6d4;
          height: 25px;
          width: 2px;
          opacity: 0;
          transition: 0.3s;
        }

        &:hover {
          color: #4bb5db;
          .b-overlay {
            opacity: 1;
          }
        }

        &.enabled {
          color: #4bb5db;
          font-weight: 500;
          .b-overlay {
            opacity: 1;
          }
        }
      }
    }

    &.expanded {
      height: auto;

      .c-content {
        display: flex;
      }
    }
  }

  .section-divider {
    margin-left: 60px;
    height: 1px;
    width: 70%;
    background: #eeeff0;
  }
}
</style>
