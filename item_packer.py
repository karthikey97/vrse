import  rectpack as rp
from random import random
import logging
# from matplotlib import pyplot as plt
# from matplotlib.patches import Rectangle

from web_scraper import get_metadata

from flask import Flask,jsonify,request,send_file

# log = logging.getLogger('werkzeug')
# log.setLevel(logging.ERROR)

app =   Flask(__name__)
packed_product_list = -1
num_packed_products = -1

avg_h = 16
avg_w = 10
var = 0.6

def pack_products(product_dict):
    shelves = [(120,60),(200,60),(120,60),(200,60)] #w,h

    num_items = len(product_dict)

    items = []
    total_h=total_w=0
    for key in product_dict:
        product = product_dict[key]
        product["height"] = avg_h * (1 + (random()-0.5)*var)
        product["width"]  = avg_w * (1 + (random()-0.5)*var)
        product["depth"]  = avg_w * (0.80 + (random()-0.5)*var*0.4)
        total_h += product["height"]
        total_w += product["width"]
        item = [product["width"]+4, product["height"]+4, product["id"]]
        items.append(item)

    avg_item_w, avg_item_h = (total_w/num_items,total_h/num_items)

    sorter = 0
    if avg_item_w>avg_item_h:
        sorter = rp.SORT_SSIDE
    else:
        sorter = rp.SORT_LSIDE
    packer = rp.newPacker(mode = rp.PackingMode.Offline, 
                        sort_algo=sorter,
                        pack_algo=rp.SkylineBl,
                        rotation=False)

    total_shelf_area = 0
    for shelf in shelves:
        packer.add_bin(*shelf)
        total_shelf_area = shelf[0]*shelf[1]
    print("Total area of shelves available:", total_shelf_area)

    total_item_area = 0
    for i,item in enumerate(items):
        packer.add_rect(*(item[:2]),rid=item[2])
        total_item_area += item[0]*item[1]
    print("Total area of items to be displayed:", total_item_area)

    packer.pack()
    packed_items = packer.rect_list()
    num_packed_items = len(packed_items)
    print(f"Packed {num_packed_items}/{num_items} items")

    packed_product_list = []
    for item in packed_items:
        product = product_dict[item[5]]
        product["x_pos"] = 2 + item[1] + item[3]/2
        product["h_pos"] = 2 + item[2] + item[4]/2
        product["shelf_id"] = item[0]
        packed_product_list.append(product)

    # fig, ax = plt.subplots()
    # plt.xlim([0, shelves[0][0]])
    # plt.ylim([0, shelves[0][1]])
    # for item in packed_items:
    #     anchor = (item[1], item[2])
    #     item_w = item[3]
    #     item_h = item[4]
    #     cw =  anchor[0] + item_w/2
    #     ch =  anchor[1] + item_h/2
    #     ax.add_patch(Rectangle(anchor, item_w, item_h, edgecolor='black', facecolor='yellow'))
    #     item_id = f"{item[5]}"
    #     ax.annotate(item_id, (cw,ch), ha='center', va='center')
    # plt.show()
    return packed_product_list

@app.route('/', methods = ['GET'])
def ReturnJSON():
    if request.method == 'GET':
        return jsonify({"prod_count":num_packed_products,"product_list":packed_product_list})

@app.route('/<img_num>', methods = ['GET'])
def ReturnImg(img_num):
    if request.method == 'GET':
        # img_path = 'ted.jpg'
        img_path = 'products_images/teddy-bear'+str(img_num)+'.jpg'
        return send_file(img_path, mimetype='image/jpg')
        

if __name__=='__main__':
    print("Getting Metadata")
    max_items = 80
    product_dict = get_metadata(max_items)
    print(f"Packing {len(product_dict)} products")
    packed_product_list = pack_products(product_dict)
    num_packed_products = len(packed_product_list)
    print(num_packed_products, "products packed")
    
    app.run(debug=True, host="0.0.0.0")
