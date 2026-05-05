using BusinessData.Property;
using CommonClassLibrary.Services;

namespace CommonClassLibrary.Models
{
    public class CommonModel
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        CommonService _service = new CommonService();

        public OutputOnDbProperty getDateTimeNow()
        {
            _resultData = _service.getDateTimeNow();
            return _resultData;
        }


        public OutputOnDbProperty getDateTimeNowByFormat(string formatDatetime)
        {
            _resultData = _service.getDateTimeNowByFormat(formatDatetime);
            return _resultData;
        }
        public OutputOnDbProperty getDateTimeNowYMD()
        {
            _resultData = _service.getDateTimeNowYMD();
            return _resultData;
        }

        public OutputOnDbProperty getYearNow(int countYear)
        {
            _resultData = _service.getYearNow(countYear);
            return _resultData;
        }
    }
}