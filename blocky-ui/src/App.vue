<template>
  <Home/>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import Home from './components/Home.vue'

export default defineComponent({
  name: 'App',
  components: {
    Home
  }
})
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  @apply text-purple-900
}

body {
  @apply bg-purple-50
}
</style>
