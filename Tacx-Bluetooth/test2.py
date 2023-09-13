import asyncio
import os
from bleak import BleakClient

from pycycling.tacx_trainer_control import TacxTrainerControl

async def run_test(adress):
    async with BleakClient(adress) as client:
        def my_page_handler(data):
            print(data)
            print(data.speed)            
        
        await client.is_connected()
        trainer = TacxTrainerControl(client=client)
        
        #trainer.set_specific_trainer_data_page_handler(my_page_handler)
        trainer.set_general_fe_data_page_handler(my_page_handler)


        await trainer.enable_fec_notifications()
        await trainer.set_basic_resistance(0)
        await asyncio.sleep(20)
        await trainer.disable_fec_notifications()



if __name__ == "__main__":
    os.environ["PYTHONASYNCIODEBUG"] = str(1)
    device_adress = "C9:BD:0B:8D:56:C7"
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run_test(device_adress))