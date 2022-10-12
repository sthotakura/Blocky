<template>
  <div v-if="isOpen">
    <form @submit.prevent="onSubmit">
      <modal @close="onModalClose">
        <template v-slot:header>
          <div class="text-xl text-left ml-2">Unblock {{ host }}</div>
        </template>
        <template v-slot:body>
          <div class="px-3 py-2">
            <input
              ref="secretInput"
              type="password"
              autocomplete="false"
              class="border border-purple-400 rounded h-8 py-1 px-3 w-full focus:outline-none focus:ring-2 focus:ring-purple-400 focus:shadow-lg disabled:bg-gray-200 disabled:cursor-not-allowed"
              v-model="secret"
              :disabled="isBusy"
              :placeholder="'Enter password to unblock ' + host"/>
          </div>
        </template>
        <template v-slot:footer>
          <div class="flex justify-between">
            <div class="py-1 px-2">
              <spinner v-if="isBusy" :size="6" :show-text="false"/>
              <div v-else-if="hasError" class="text-red-900">
                {{ error }}
              </div>
            </div>
            <div>
              <button
                class="disabled:text-gray-400 uppercase disabled:border-gray-400 disabled:cursor-not-allowed hover:text-white hover:bg-purple-700 rounded font-bold px-3 py-1 mr-4 ease-linear transition-all duration-150"
                type="reset"
                :disabled="isBusy"
                @click="onModalClose">
                Cancel
              </button>
              <button
                class="disabled:text-gray-400 uppercase disabled:border-gray-400 disabled:cursor-not-allowed hover:text-white hover:bg-purple-700 rounded font-bold px-3 py-1 ease-linear transition-all duration-150"
                type="submit"
                :disabled="isBusy">
                Unblock
              </button>
            </div>
          </div>
        </template>
      </modal>
    </form>
  </div>
</template>

<script lang="ts">
import {defineComponent, onMounted, PropType, ref} from "vue";
import Modal from "./Modal.vue";
import {blockyService} from "../blockyService";
import Spinner from "./Spinner.vue";

export default defineComponent({
  name: "SecretInputModal",
  components: {Spinner, Modal},
  props: {
    isOpen: {type: Boolean as PropType<boolean>, default: false},
    host: {type: String as PropType<string>, required: true}
  },
  setup(props, context) {
    const secret = ref("")
    const isBusy = ref(false)
    const hasError = ref(false)
    const error = ref("")
    const secretInput = ref<HTMLInputElement>()

    const onSubmit = async () => {
      if (secret.value) {
        isBusy.value = true
        hasError.value = false
        try {
          await blockyService.unblock(props.host, secret.value)
          isBusy.value = false
          secret.value = ""
          context.emit('secretInputModelSubmitted')
        } catch (e: any) {
          hasError.value = true
          error.value = e.message
          isBusy.value = false
        }
      }
    }

    const onModalClose = () => {
      if (isBusy.value) {
        return
      }
      secret.value = ""
      context.emit('secretInputModalCancelled')
    }

    onMounted(() => setTimeout(() => {
      if (secretInput.value) {
        (secretInput.value as HTMLInputElement).focus()
      }
    }, 250))

    return {
      onModalClose,
      onSubmit,
      secret,
      isBusy,
      hasError,
      error
    }
  }
})
</script>

<style scoped>

</style>