<template>
  <div class="field">
    <section id="spacer"></section>

    <span
      class="field-char"
      v-for="(char,index) in currentValue"
      :key="`char-${index}`"
    >{{char == ' ' ? '&nbsp;' : char}}</span>

  </div>
</template>

<script>
export default {
  props: ['value', 'fieldName'],
  data() {
    return {
      currentValue: ''
    }
  },
  created() {
    this.currentValue = this.value

    this.$bus.$on('decrypt', (data) => {
      if (data.fieldName != this.fieldName) return
      this.animateOut(data.newData)
    })
  },
  methods: {
    animateOut(newData) {
      var elementList = document.querySelectorAll('.field-char')
      for (var i = 0; i < elementList.length; i++) {
        elementList[i].style.transform = `translate(0px,-${Math.floor(Math.random() * Math.floor(100))}px) `
        elementList[i].style.opacity = 0
      }

      setTimeout(() => {
        var elementList = document.querySelectorAll('.field')
        for (var i = 0; i < elementList.length; i++) {
          elementList[i].style.border = '1px solid white'
        }

        elementList = document.querySelectorAll('.field-char')
        for (var i = 0; i < elementList.length; i++) {
          elementList[i].style.transform = `translate(0px,0px) `
          elementList[i].style.opacity = 1
        }
        this.currentValue = newData
      }, 1200)
    }
  }
}
</script>

<style lang="scss">
$primary-color: #333742;

#spacer {
  width: 14px;
}

.field {
  font-size: 13px;
  border: 1px solid #333742;
  height: 45px;
  width: 300px;
  display: flex;
  align-items: center;
  margin-bottom: 20px;
  overflow: hidden;
  transition: 1s ease-in-out;
}

.field-char {
  margin-right: 1px;
  user-select: none;
  transition: 1s ease-in-out;
}
</style>
