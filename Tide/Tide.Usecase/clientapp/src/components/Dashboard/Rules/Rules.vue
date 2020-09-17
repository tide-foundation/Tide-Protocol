<template>
  <div id="rules">
    <div id="rules-topbar">
      <div v-if="selectedRule != null">
        <button @click="$parent.selectedRule = null">BACK TO RULES</button>
      </div>
    </div>
    <RuleList v-if="selectedRule == null" :rules="rules"></RuleList>
    <Rule v-if="selectedRule != null" :rule="selectedRule"></Rule>
  </div>
</template>

<script>
import RuleList from "./RuleList.vue";
import Rule from "./Rule.vue";
export default {
    components: { RuleList, Rule },
    data: function() {
        return {
            rules: [],
            seededIndex: 0,
            selectedRule: null
        };
    },
    created() {
        this.seedRules();
    },
    methods: {
        seedRules() {
            this.rules = [
                {
                    id: this.$helper.generateUniqueId(),
                    name: "My rule",
                    tag: "Email",
                    destination: "Vendor",
                    action: "Allow",
                    automation: true,
                    state: "Active",
                    expiration: new Date(),
                    conditions: [
                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "And",
                            field: null,
                            operator: "=",
                            value: "field2",
                            level: 0
                        },

                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "Or",
                            field: null,
                            operator: ">",
                            value: "field3",
                            level: 0
                        },
                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "Or",
                            field: null,
                            operator: ">",
                            value: "field3",
                            level: 0
                        },
                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "And",
                            field: null,
                            operator: "=",
                            value: "field2",
                            level: 0
                        },
                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "And",
                            field: null,
                            operator: "=",
                            value: "field2",
                            level: 0
                        },
                        {
                            id: this.$helper.generateUniqueId(),
                            index: this.seededIndex++,
                            selected: false,
                            union: "And",
                            field: null,
                            operator: "=",
                            value: "field2",
                            level: 0
                        }
                    ]
                }
            ];
        }
    }
};
</script>

<style lang="scss" scoped>
#rules {
    width: 100%;
    padding: 10px;
    #rules-topbar {
        width: 100%;
        margin-bottom: 20px;
    }
}
</style>