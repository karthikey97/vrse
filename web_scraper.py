import json
from math import ceil

def get_metadata(max_items):
    with open('product_output.jsonl') as f:
        lines = f.readlines()

    with open('search_output.jsonl') as f1:
        search_lines = f1.readlines()

    pcnt = 0
    product_dict = {}
    for product,search in zip(lines, search_lines):
        try:
            name = json.loads(product)['name']
            prod_summary = (json.loads(product)['product_addl_info'])
            price = json.loads(search[:-3])['price'][1:]
            for detail in prod_summary:
                if ("imension" in detail['info']):
                    dim = detail['value'][:-7].split(' x ')
                if (detail['info'] == 'ASIN'):
                    ID = detail['value']
                if (detail['info'] == 'Customer Reviews'):
                    rating = detail['value'][-18:][:3]
        except:
            continue
        
        if (ID in product_dict): print(ID,"collision")
        product_dict[ID] = {
            "id":ID,
            "name":name,
            "rating":rating,
            "price":float(price),
            "height":ceil(float(dim[2])),
            "width":ceil(float(dim[1])),
            "depth":ceil(float(dim[0]))
        }
        
        pcnt += 1
        if pcnt==max_items: break

    return product_dict