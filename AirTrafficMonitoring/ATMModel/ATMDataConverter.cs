﻿using System.Collections.Generic;
using System.Linq;

namespace ATMModel
{
    public class ATMDataConverter : IATMDataConverter
    {
        private readonly IATMAngleConverter _angle;
        private readonly IATMVelocityConverter _velocity;
        private static List<IATMTransponderData> _locaDataItems = new List<IATMTransponderData>();
        public ATMDataConverter(IATMAngleConverter angle, IATMVelocityConverter velocity)
        {
            _angle = angle;
            _velocity = velocity;
        }

        public List<IATMTransponderData> Convert(List<string> list) 
        {
            var currentItems = new List<IATMTransponderData>();
            foreach (var data in list.Select(item => item.Split(';')))
            {
                var coordinate = new ATMCoordinate(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
                if(!coordinate.Validate) continue;

                var newItem =new ATMTransponderData(
                    data[0],
                    int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]),
                    data[4]
                    );

                if (!_locaDataItems.Exists(t => t.Tag == newItem.Tag))
                {
                    currentItems.Add(newItem);
                    continue;
                }

                var oldItem = _locaDataItems.First(t => t.Tag == newItem.Tag);
                newItem.CompassCourse = (int)_angle.Convert(oldItem.Coordinate, newItem.Coordinate);
                newItem.HorizontalVelocity = (int)_velocity.Convert(oldItem.Coordinate, newItem.Coordinate, oldItem.Timestamp, newItem.Timestamp);
                currentItems.Add(newItem);
            }
            _locaDataItems = currentItems;
            return _locaDataItems;
            
        }
    }
}