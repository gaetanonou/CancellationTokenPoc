<template>
  <v-container>
    <v-layout row wrap>
        <v-flex>
            <h1>Todos</h1>
            <v-text-field placeholder="Search" solo @input="getTodos"></v-text-field>
            <v-text-field placeholder="Search debounce" solo @input="getTodosDebounced"></v-text-field>
            <v-text-field placeholder="Search cancel" solo @input="getTodosCancellableDebounced"></v-text-field>
            <v-text-field placeholder="Search watch" solo v-model="searchTerms"></v-text-field>
            <v-progress-linear v-show="loading" :indeterminate="true"></v-progress-linear>
            <v-list v-show="!loading && todos.length" two-line>
                <template v-for="(todo, index) in todos">
              <v-divider :key="index + '_devider'"></v-divider>
              <v-list-tile :key="index + '_list-title'">
                  <v-list-tile-content>
                      <v-list-tile-title v-html="todo.title"></v-list-tile-title>
                  </v-list-tile-content>
              </v-list-tile>
          </template>
        </v-list>
        </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import TodosService from '../services/TodosService';
import { debounce } from 'lodash';
import axios from 'axios';

export default {
    data: () => ({
        loading: false,
        searchTerms: '',
        todos: [],
        getTodosCancelTokenSource: null,
    }),
    async mounted(){
       await this.getTodos();
    },
    methods:{
        async getTodos(value, cancelTokenSource){
            console.log(`New value: ${value}`);
            this.loading = true;
            try {
                this.todos = await TodosService.getTodos(value, cancelTokenSource);
                this.loading = false;

            } catch(error){
                if (axios.isCancel(error)) {
                    console.warn('Request was cancelled');
                } else {
                    // handle error
                    this.loading = false;
                }
            }
        },
        getTodosDebounced: debounce(async function(value){
            console.log('getTodosDebounced');
            this.getTodos(value);
        }, 500),
        async getTodosCancellable(value) {
            if(this.getTodosCancelTokenSource){
                this.getTodosCancelTokenSource.cancel();
            }
        
            this.getTodosCancelTokenSource = axios.CancelToken.source();
            await this.getTodos(value, this.getTodosCancelTokenSource);
        },
        getTodosCancellableDebounced: debounce(async function(value) {
            console.log('getTodosCancellable');
            await this.getTodosCancellable(value)        
        }, 500)
    },
    watch: {
     searchTerms:debounce(async function (value, oldValue) {
         console.log('Watch search terms');
         console.log(`Old value: ${oldValue}`);
            if(value !== oldValue){
                await this.getTodosCancellable(value)
                }
            }, 500)
     }
  }
</script>
