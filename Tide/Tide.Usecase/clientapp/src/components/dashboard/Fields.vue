<template>
  <section id="fields-container">
    <div
      class="box"
      v-for="(fieldSection, index) in fields"
      :key="index"
      :style="'border-top: 2px solid ' + fieldSection.color"
    >
      <h4>{{ fieldSection.section }}</h4>

      <section
        class="chk col-xs-6 col-sm-4"
        v-for="(field, fieldIndex) in fieldSection.fields"
        :key="fieldIndex"
      >
        <button
          class="field-btn"
          :class="{
            'disabled-field': $store.getters.disabledFields.includes(
              field + fieldSection.section
            )
          }"
          @click="alter(field + fieldSection.section)"
        >
          {{ field }}
        </button>
      </section>
    </div>
  </section>
</template>

<script>
export default {
  components: {},
  data() {
    return {
      fields: [
        {
          section: "Personal",
          color: "orange",
          fields: [
            "First Name",
            "Middle Name",
            "Last Name",
            "Phone Number",
            "Email Address",
            "Date of Birth"
          ]
        },
        {
          section: "Addresses",
          color: "#8fbe00",
          fields: ["Street", "Suburb", "State", "Postcode"]
        },
        {
          section: "Employment",
          color: "#01acfe",
          fields: ["Employer", "Phone Number", "Email Address", "Pay"]
        },
        {
          section: "Credit",
          color: "#CC2A41",
          fields: ["Credit Card", "Personal Loan", "Other Loan"]
        }
      ]
    };
  },
  created() {},
  methods: {
    alter(item) {
      var array = this.$store.getters.disabledFields;
      if (this.$store.getters.disabledFields.includes(item)) {
        array = array.filter(v => v !== item);
      } else {
        array.push(item);
      }
      this.$store.commit("updatedisabledFields", array);
    }
  }
};
</script>

<style lang="scss" scoped>
#fields-container {
  display: flex;
  align-items: center;
  flex-direction: column;
}

.box {
  margin-bottom: 20px;
  width: 100%;
  padding: 10px;

  section {
    display: flex;
    flex-direction: row;
    align-items: center;
  }
}

.field-btn {
  width: 150px;
  height: 40px;
  background-color: #01acfe;
  color: white;
  border: 0px;
  margin-bottom: 10px;
  border-radius: 2px;
}

.field-btn:hover {
  background-color: #556270;
}

.disabled-field {
  background-color: #778492;
}
</style>
