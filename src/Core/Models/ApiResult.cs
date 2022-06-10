using System.Collections.Generic;

namespace Powertoys.Run.Devbox.Core.Models
{
  public class ApiResult
  {
    public int total_count { get; set; }
    public bool incomplete_results { get; set; }
    public List<ApiResultRepo> items { get; set; }
  }
}
