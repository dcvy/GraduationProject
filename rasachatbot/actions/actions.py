# This files contains your custom actions which can be used to run
# custom Python code.
#
# See this guide on how to implement these action:
# https://rasa.com/docs/rasa/core/actions/#custom-actions/
# This is a simple example for a custom action which utters "Hello World!"
import json
import logging
import random
from token import AT
from typing import Dict, Text, Any, List, Union, Optional
from rasa_sdk.types import DomainDict
import psycopg2
from rasa_sdk import Action, FormValidationAction
from rasa_sdk.executor import CollectingDispatcher
from rasa_sdk.events import SlotSet, FollowupAction, AllSlotsReset, EventType, ConversationPaused, UserUtteranceReverted
import requests
from rasa_sdk import Tracker


class ActionGetProduct(Action):
    def name(self):
        return 'action_product'

    def run(self, dispatcher: CollectingDispatcher, tracker: Tracker, domain):
        url = 'https://localhost:5001/api/Products'
        headers = {'Content-Type': 'application/json'}
        request = requests.get(url, headers=headers, verify=False)

        if request.status_code == 200:
            try:
                products = request.json()

                # Check if there are products
                if products:
                    # Show the message only once
                    dispatcher.utter_message(text="The available products are:")

                    # Iterate through each product and send a separate message for each
                    for product in products:
                        response_text = f"Name: {product['name']}<br>Price: ${product['price']}"

                        # Constructing the full image URL including 'https://localhost:5001'
                        image_url = f"https://localhost:5001{product['imageUrl']}"

                        # Send a message for each product
                        dispatcher.utter_message(text=response_text)
                        dispatcher.utter_message(image=image_url)

                else:
                    dispatcher.utter_message(text='No products available.')

                return []
            except json.JSONDecodeError:
                dispatcher.utter_message(text='Failed to decode JSON response from the API.')
                return []
        else:
            dispatcher.utter_message(text='I am facing some issues, please try again later!!!')
            return []
        

class ActionSearchProduct(Action):
    def name(self):
        return 'action_search_product'

    def run(self, dispatcher: CollectingDispatcher, tracker: Tracker, domain):
        # Extract the value of the 'category' entity
        category_entity = tracker.get_slot('category')

        if category_entity:
            # Make a request to your ASP.NET API with the category parameter
            url = f'https://localhost:5001/api/Products?CateItem={category_entity}'
            headers = {'Content-Type': 'application/json'}

            try:
                # Make the request
                request = requests.get(url, headers=headers, verify=False)

                # Check the response status
                if request.status_code == 200:
                    products = request.json()

                    # Check if there are products
                    if products:
                        dispatcher.utter_message(text=f"The available {category_entity} products are:")

                        # Iterate through each product and send a separate message for each
                        for product in products:
                            response_text = f"Name: {product['name']}<br>Price: ${product['price']}"
                            image_url = f"https://localhost:5001{product['imageUrl']}"
                            dispatcher.utter_message(text=response_text)
                            dispatcher.utter_message(image=image_url)

                    else:
                        dispatcher.utter_message(text=f'No {category_entity} products available.')

                else:
                    dispatcher.utter_message(text=f"Failed to retrieve products. Status code: {request.status_code}")

            except requests.RequestException as e:
                dispatcher.utter_message(text=f"Failed to connect to the API. Error: {str(e)}")

        else:
            dispatcher.utter_message(text='No category specified.')
        
        return []





class ActionGetCategories(Action):
    def name(self):
        return 'action_categories_list'

    def run(self, dispatcher, tracker, domain):
        url = 'https://localhost:5001/api/CateItems'
        headers = {'Content-Type': 'application/json'}
        request = requests.get(url, headers=headers, verify=False)

        if request.status_code == 200:
            try:
                categories_data = request.json()
                categories = [item["name"] for item in categories_data]

                # Create a user-friendly response
                response = "The available categories are: " + ", ".join(categories)
                dispatcher.utter_message(text=response)

                return []
            except json.JSONDecodeError:
                dispatcher.utter_message(text='Failed to decode JSON response from the API.')
                return []
        else:
            dispatcher.utter_message(text='I am facing some issues, please try again later!!!')
            return []




class ActionDefaultFallback(Action):
    def name(self):
        return 'action_default_fallback'

    def run(self, dispatcher, tracker, domain):
        # Your fallback logic here
        dispatcher.utter_message(text="I'm sorry, I didn't understand that.")
        return []